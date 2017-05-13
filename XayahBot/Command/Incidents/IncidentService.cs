#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using XayahBot.API.Riot;
using XayahBot.API.Riot.Model;
using XayahBot.Database.DAO;
using XayahBot.Database.Error;
using XayahBot.Database.Model;
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
        private readonly MessagesDAO _messagesDao = new MessagesDAO();
        private readonly IncidentsDAO _incidentsDao = new IncidentsDAO();
        private readonly IncidentSubscriberDAO _incidentSubscriberDao = new IncidentSubscriberDAO();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
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
                if (!this._isRunning && this._incidentSubscriberDao.HasAnySubscriber())
                {
                    this._isRunning = true;
                    Task.Run(() => RunAsync());
                    Logger.Info("IncidentService started.");
                }
            }
            finally
            {
                this._lock.Release();
            }
        }

        private async Task RunAsync()
        {
            try
            {
                bool init = true;
                bool processed = false;
                while (this._isRunning)
                {
                    int interval = 5;
                    if (DateTime.UtcNow.Minute % interval == 0 || init)
                    {
                        if (!processed)
                        {
                            init = false;
                            processed = true;
                            await CheckStatusApiAsync();
                        }
                    }
                    else
                    {
                        processed = false;
                    }
                    if (!this._incidentSubscriberDao.HasAnySubscriber())
                    {
                        this._isRunning = false;
                    }
                    await Task.Delay(10000);
                }
            }
            finally
            {
                this.Stop();
            }
        }

        private async Task CheckStatusApiAsync()
        {
            List<IncidentData> incidents = new List<IncidentData>();
            RiotStatusApi statusEuw = new RiotStatusApi(Region.EUW);
            RiotStatusApi statusNa = new RiotStatusApi(Region.NA);
            RiotStatusApi statusEune = new RiotStatusApi(Region.EUNE);
            incidents.AddRange(this.AnalyzeData(await statusEuw.GetStatusAsync()));
            incidents.AddRange(this.AnalyzeData(await statusNa.GetStatusAsync()));
            incidents.AddRange(this.AnalyzeData(await statusEune.GetStatusAsync()));
            await this.ProcessCurrentIncidentsAsync(incidents);
            await this.ProcessSolvedIncidentsAsync(incidents);
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
                if (this.IsNewUpdate(incident.Id, incident.LastUpdate))
                {
                    await this.DeletePostsAsync(incident.Id);
                }
                await this.PostIncidentAsync(incident);
            }
        }

        private bool IsNewUpdate(long incidentId, DateTime lastUpdate)
        {
            try
            {
                TIncident entry = this._incidentsDao.GetSingle(incidentId);
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
                TIncident entry = this._incidentsDao.GetSingle(incidentId);
                foreach (TMessage message in entry.Messages)
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
                }
                await this._messagesDao.RemoveByIncidentIdAsync(entry.IncidentId);
            }
            catch (NotExistingException)
            {
            }
        }

        private async Task PostIncidentAsync(IncidentData incident)
        {
            DiscordFormatEmbed message = this.CreateEmbed(incident);
            TIncident entry = this.RetrieveDbIncident(incident);
            foreach (TIncidentSubscriber subscriber in this.GetInterestedSubscriber(entry))
            {
                IMessageChannel channel = this._client.GetChannel(subscriber.ChannelId) as IMessageChannel;
                try
                {
                    IUserMessage postedMessage = await channel.SendMessageAsync("", false, message.ToEmbed());
                    await this.SaveMessageIdAsync(entry, postedMessage);
                }
                catch (HttpException)
                {
                    // If your permission got revoked you can't access that channel anymore and it throws HttpException
                }
            }
        }

        private DiscordFormatEmbed CreateEmbed(IncidentData incident)
        {
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .AppendDescription($"{incident.Region} | {incident.Service} | {incident.Status}", AppendOption.Bold);
            foreach (UpdateData update in incident.Updates)
            {
                message.AppendDescription(Environment.NewLine)
                    .AppendDescription($"{update.Severity} - {update.UpdateTime}", AppendOption.Italic)
                    .AppendDescription(Environment.NewLine)
                    .AppendDescription(update.Text);
            }
            return message;
        }

        private TIncident RetrieveDbIncident(IncidentData incident)
        {
            TIncident entry = null;
            try
            {
                entry = this._incidentsDao.GetSingle(incident.Id);
            }
            catch (NotExistingException)
            {
                entry = new TIncident
                {
                    IncidentId = incident.Id,
                };
            }
            entry.LastUpdate = incident.LastUpdate;
            return entry;
        }

        private List<TIncidentSubscriber> GetInterestedSubscriber(TIncident entry)
        {
            List<TIncidentSubscriber> subscriberList = this._incidentSubscriberDao.GetAll();
            List<TIncidentSubscriber> reducedList = new List<TIncidentSubscriber>(subscriberList);
            foreach (TIncidentSubscriber subscriber in subscriberList)
            {
                if (entry.Messages.Where(x => x.ChannelId.Equals(subscriber.ChannelId)).Count() > 0)
                {
                    reducedList.Remove(subscriber);
                }
            }
            return reducedList;
        }

        private async Task SaveMessageIdAsync(TIncident entry, IUserMessage postedMessage)
        {
            TMessage message = new TMessage
            {
                ChannelId = postedMessage.Channel.Id,
                Incident = entry,
                MessageId = postedMessage.Id
            };
            entry.Messages.Add(message);
            await this._incidentsDao.SaveAsync(entry);
        }

        private async Task ProcessSolvedIncidentsAsync(List<IncidentData> incidents)
        {
            List<TIncident> dbIncidents = this._incidentsDao.GetAll();
            foreach (TIncident dbIncident in dbIncidents)
            {
                if (incidents.Where(x => x.Id.Equals(dbIncident.IncidentId)).Count() == 0)
                {
                    await this.DeletePostsAsync(dbIncident.IncidentId);
                    await this._incidentsDao.RemoveByIncidentIdAsync(dbIncident.IncidentId);
                }
            }
        }

        public void Stop()
        {
            if (this._isRunning)
            {
                this._isRunning = false;
                Logger.Info("IncidentService stopped.");
            }
        }
    }
}
