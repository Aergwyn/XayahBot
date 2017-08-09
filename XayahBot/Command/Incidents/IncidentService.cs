using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using XayahBot.API.Error;
using XayahBot.API.Riot;
using XayahBot.API.Riot.Model;
using XayahBot.Database.DAO;
using XayahBot.Database.Model;
using XayahBot.Error;
using XayahBot.Extension;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Incidents
{
    public class IncidentService
    {
        private static IncidentService _instance;

        public static IncidentService GetInstance(DiscordSocketClient client)
        {
            if (_instance == null)
            {
                _instance = new IncidentService(client);
            }
            return _instance;
        }

        // ---

        private readonly DiscordSocketClient _client;
        private readonly IncidentDAO _incidentDAO = new IncidentDAO();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly IncidentMessageDAO _incidentMessageDAO = new IncidentMessageDAO();
        private readonly IncidentSubscriberDAO _incidentSubscriberDAO = new IncidentSubscriberDAO();
        private Task _process;
        private bool _isRunning = false;

        private IncidentService(DiscordSocketClient client)
        {
            this._client = client;
        }

        public async Task StartAsync()
        {
            await this._lock.WaitAsync();
            try
            {
                if (this.IsEnabled() && !this._isRunning && this._incidentSubscriberDAO.HasSubscriber())
                {
                    this._process = Task.Run(() => this.RunAsync());
                    Logger.Info($"{nameof(IncidentService)} started.");
                }
            }
            finally
            {
                this._lock.Release();
            }
        }

        private bool IsEnabled()
        {
            string value = Property.IncidentDisabled.Value;
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return false;
        }

        private async Task RunAsync()
        {
            bool init = true;
            bool processed = false;
            this._isRunning = true;
            while (this._isRunning)
            {
                int interval = int.Parse(Property.IncidentCheckInterval.Value);
                if (DateTime.UtcNow.Minute % interval == 0 || init)
                {
                    if (!processed)
                    {
                        init = false;
                        processed = true;
                        await this.CheckStatusApiAsync();
                    }
                }
                else
                {
                    processed = false;
                }
                if (this.IsEnabled() && this._incidentSubscriberDAO.HasSubscriber())
                {
                    await Task.Delay(new TimeSpan(0, 0, 10));
                }
                else
                {
#pragma warning disable 4014
                    this.StopAsync();
#pragma warning restore 4014
                }
            }
        }

        private async Task CheckStatusApiAsync()
        {
            List<RiotStatus> statusApis = new List<RiotStatus>{
                new RiotStatus(Region.EUW),
                new RiotStatus(Region.NA),
                new RiotStatus(Region.EUNE)
            };
            foreach (RiotStatus statusApi in statusApis)
            {
                List<IncidentData> incidents = new List<IncidentData>();
                try
                {
                    ShardStatusDto shardStatus = await statusApi.GetStatusAsync();
                    incidents.AddRange(this.AnalyzeData(shardStatus));
                    await this.ProcessCurrentIncidentsAsync(incidents);
                    await this.ProcessSolvedIncidentsAsync(incidents);
                }
                catch (ErrorResponseException ex)
                {
                    Logger.Error($"The {statusApi.GetRegion()} Status-API returned an error.", ex);
                }
            }
        }

        private List<IncidentData> AnalyzeData(ShardStatusDto status)
        {
            List<IncidentData> incidents = new List<IncidentData>();
            foreach (ServiceDto service in status.Services)
            {
                foreach (IncidentDto incident in service.Incidents.Where(x => x.Active && x.Updates.Count > 0).ToList())
                {
                    IncidentData incidentData = new IncidentData
                    {
                        Id = incident.Id,
                        Region = status.Region,
                        Service = service.Name,
                        Status = service.Status
                    };
                    foreach (UpdateDto update in incident.Updates)
                    {
                        DateTime.TryParse(update.UpdateTime, out DateTime updateTime);
                        UpdateData updateData = new UpdateData
                        {
                            Severity = update.Severity.ToUpper(),
                            Text = update.Content,
                            UpdateTime = updateTime
                        };
                        if (incidentData.LastUpdate == null || updateTime.Ticks > incidentData.LastUpdate.Ticks)
                        {
                            incidentData.LastUpdate = updateTime;
                        }
                        incidentData.Updates.Add(updateData);
                    }
                    incidents.Add(incidentData);
                }
            }
            return incidents;
        }

        private async Task ProcessCurrentIncidentsAsync(List<IncidentData> incidents)
        {
            foreach (IncidentData incident in incidents)
            {
                if (this.GotNewUpdate(incident.Id, incident.LastUpdate))
                {
                    await this.DeletePostsAsync(incident.Id);
                }
                await this.PostIncidentAsync(incident);
            }
        }

        private bool GotNewUpdate(long incidentId, DateTime lastUpdate)
        {
            try
            {
                TIncident entry = this._incidentDAO.Get(incidentId);
                if (lastUpdate.Ticks > entry.LastUpdate.Ticks)
                {
                    return true;
                }
            }
            catch (NotExistingException)
            {
            }
            return false;
        }

        private async Task DeletePostsAsync(long incidentId)
        {
            try
            {
                TIncident dbIncident = this._incidentDAO.Get(incidentId);
                foreach (TIncidentMessage message in dbIncident.Messages)
                {
                    IMessageChannel channel = this._client.GetChannel(message.ChannelId) as IMessageChannel;
                    try
                    {
                        IMessage postedMessage = await channel.GetMessageAsync(message.MessageId);
                        await postedMessage?.DeleteAsync();
                    }
                    catch (NullReferenceException)
                    {
                        // If a message got deleted it throws NullReferenceException if you try to access it. weird.
                    }
                    catch (HttpException)
                    {
                        // If your permission got revoked you can't access that message anymore and it throws HttpException
                    }
                    await this._incidentMessageDAO.RemoveAsync(message);
                }
            }
            catch (NotExistingException)
            {
            }
        }

        private async Task PostIncidentAsync(IncidentData incident)
        {
            TIncident dbIncident = this.GetDbIncident(incident);
            foreach (TIncidentSubscriber subscriber in this.GetInterestedSubscriber(dbIncident))
            {
                IMessageChannel channel = this._client.GetChannel(subscriber.ChannelId) as IMessageChannel;
                try
                {
                    IUserMessage postedMessage = await channel.SendEmbedAsync(this.CreateEmbed(incident));
                    await this.SaveMessageAsync(dbIncident, postedMessage);
                }
                catch (HttpException)
                {
                    // If your permission got revoked you can't access that channel anymore and it throws HttpException
                }
            }
        }

        private TIncident GetDbIncident(IncidentData incident)
        {
            TIncident newDbIncident = null;
            try
            {
                newDbIncident = this._incidentDAO.Get(incident.Id);
            }
            catch (NotExistingException)
            {
                newDbIncident = new TIncident
                {
                    IncidentId = incident.Id,
                };
            }
            newDbIncident.LastUpdate = incident.LastUpdate;
            return newDbIncident;
        }

        private List<TIncidentSubscriber> GetInterestedSubscriber(TIncident dbIncident)
        {
            List<TIncidentSubscriber> subscriberList = this._incidentSubscriberDAO.GetAll();
            List<TIncidentSubscriber> reducedList = new List<TIncidentSubscriber>(subscriberList);
            foreach (TIncidentSubscriber subscriber in subscriberList)
            {
                if (dbIncident.Messages.Where(x => x.ChannelId.Equals(subscriber.ChannelId)).Count() > 0)
                {
                    reducedList.Remove(subscriber);
                }
            }
            return reducedList;
        }

        private FormattedEmbedBuilder CreateEmbed(IncidentData incident)
        {
            FormattedEmbedBuilder message = new FormattedEmbedBuilder()
                .AppendDescription($"{incident.Region} | {incident.Service} | {incident.Status}", AppendOption.Bold);
            foreach (UpdateData update in incident.Updates)
            {
                message
                    .AppendDescriptionNewLine()
                    .AppendDescription($"{update.Severity} - {update.UpdateTime}", AppendOption.Italic)
                    .AppendDescriptionNewLine()
                    .AppendDescription(update.Text);
            }
            return message;
        }

        private async Task SaveMessageAsync(TIncident dbIncident, IUserMessage postedMessage)
        {
            TIncidentMessage message = new TIncidentMessage
            {
                ChannelId = postedMessage.Channel.Id,
                Incident = dbIncident,
                MessageId = postedMessage.Id
            };
            dbIncident.Messages.Add(message);
            await this._incidentDAO.SaveAsync(dbIncident);
        }

        private async Task ProcessSolvedIncidentsAsync(List<IncidentData> incidents)
        {
            List<TIncident> dbIncidents = this._incidentDAO.Get();
            foreach (TIncident dbIncident in dbIncidents)
            {
                if (incidents.Where(x => x.Id.Equals(dbIncident.IncidentId)).Count() == 0)
                {
                    await this.DeletePostsAsync(dbIncident.IncidentId);
                    await this._incidentDAO.RemoveAsync(dbIncident);
                }
            }
        }

        public async Task StopAsync()
        {
            Logger.Info($"Requested stop for {nameof(IncidentService)}.");
            this._isRunning = false;
            if (this._process != null)
            {
                await this._process;
            }
        }
    }
}
