#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using XayahBot.API.Riot;
using XayahBot.API.Riot.Model;
using XayahBot.Database.DAO;
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

        private DiscordSocketClient _client;
        private readonly IncidentsDAO _incidentsDao = new IncidentsDAO();
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
                if (!this._isRunning && this._incidentsDao.HasSubscriber())
                {
                    this._isRunning = true;
                    Task.Run(() => Run());
                    Logger.Info("IncidentService started.");
                }
            }
            finally
            {
                this._lock.Release();
            }
        }

        private async Task Run()
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
                            await CheckStatusApi();
                        }
                    }
                    else
                    {
                        processed = false;
                    }
                    if (!this._incidentsDao.HasSubscriber())
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

        private async Task CheckStatusApi()
        {
            RiotStatusApi statusEuw = new RiotStatusApi(Region.EUW);
            RiotStatusApi statusNa = new RiotStatusApi(Region.NA);
            await this.AnalyzeDataAsync(await statusEuw.GetStatusAsync());
            await this.AnalyzeDataAsync(await statusNa.GetStatusAsync());
        }

        private async Task AnalyzeDataAsync(ShardStatusDto status)
        {
            foreach (ServiceDto service in status.Services)
            {
                foreach (IncidentDto incident in this.GetValidIncidents(service))
                {
                    bool postMessage = false;
                    DiscordFormatEmbed message = new DiscordFormatEmbed();
                    for (int updatePos = 0; updatePos < incident.Updates.Count; updatePos++)
                    {
                        UpdateDto update = incident.Updates.ElementAt(updatePos);
                        DateTime.TryParse(update.UpdateTime, out DateTime updateTime);
                        if (updatePos == 0)
                        {
                            message.AppendDescription($"{status.Name} | {service.Name} | {service.Status}", AppendOption.Bold);
                        }
                        message.AppendDescription(Environment.NewLine)
                            .AppendDescription($"{update.Severity.ToUpper()} - {updateTime}")
                            .AppendDescription(Environment.NewLine)
                            .AppendDescription(update.Content);
                        postMessage = true;
                    }
                    if (postMessage)
                    {
                        // checkif already posted
                        if (false)
                        {
                            // edit
                        }
                        else
                        {
                            await this.PostData(message);
                            // save post and metadata
                        }
                    }
                }
            }
        }

        private List<IncidentDto> GetValidIncidents(ServiceDto service)
        {
            return service.Incidents.Where(x => x.Active && x.Updates.Count > 0).ToList();
        }

        private async Task PostData(DiscordFormatEmbed message)
        {
            List<TIncidentSubscriber> subscriberList = this._incidentsDao.GetSubscriber();
            foreach (TIncidentSubscriber subscriber in subscriberList)
            {
                IMessageChannel channel = ResponseHelper.GetChannel(this._client, subscriber.ChannelId);
                await channel.SendMessageAsync("", false, message.ToEmbed());
            }
        }

        public void Stop()
        {
            this._isRunning = false;
            Logger.Info("IncidentService stopped.");
        }
    }
}
