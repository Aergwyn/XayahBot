using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using XayahBot.API.Riot;
using XayahBot.API.Riot.Model;

namespace XayahBot.Service
{
    public class RiotStatusService
    {
        private static RiotStatusService _instance;

        //

        public static Task StartAsync(DiscordSocketClient client)
        {
            if (_instance == null)
            {
                _instance = new RiotStatusService(client);
                Task.Run(() => _instance.Start());
            }
            return Task.CompletedTask;
        }

        public static void Stop()
        {
            if (_instance != null)
            {
                _instance._running = false;
            }
        }

        //

        private DiscordSocketClient _client;
        private bool _running;

        //

        private RiotStatusService(DiscordSocketClient client)
        {
            this._client = client;
            this._running = false;
        }

        //

        private async Task Start()
        {
            if (!this._running)
            {
                this._running = true;
                bool _checked = false;
                RiotStatusApi statusEuw = new RiotStatusApi(Region.EUW);
                RiotStatusApi statusNa = new RiotStatusApi(Region.NA);
                while (this._running)
                {
                    if (DateTime.UtcNow.Minute % 5 == 0)
                    {
                        if (!_checked)
                        {
                            AnalyzeData(await statusEuw.GetStatusAsync());
                            AnalyzeData(await statusNa.GetStatusAsync());
                            _checked = true;
                        }
                    }
                    else
                    {
                        _checked = false;
                    }
                    if (this._running)
                    {
                        await Task.Delay(20000);
                    }
                }
            }
        }

        private void AnalyzeData(ShardStatusDto status)
        {
            ISocketMessageChannel testChannel = this._client.GetChannel(301030133014200320) as ISocketMessageChannel;
            string message = string.Empty;
            for (int servicePos = 0; servicePos < status.Services.Count; servicePos++)
            {
                ServiceDto service = status.Services.ElementAt(servicePos);
                List<UpdateDto> updates = this.GetActiveUpdates(service);
                for (int updatePos = 0; updatePos < updates.Count(); updatePos++)
                {
                    if (string.IsNullOrWhiteSpace(message))
                    {
                        message += $"__Status {status.Name}__{Environment.NewLine}";
                    }
                    if (updatePos == 0)
                    {
                        message += $"{service.Name} | {service.Status}{Environment.NewLine}";
                    }
                    else
                    {
                        message += Environment.NewLine;
                    }
                    UpdateDto update = updates.ElementAt(updatePos);
                    DateTime.TryParse(update.UpdateTime, out DateTime updateTime);
                    message += $"--- {update.Id} - {update.Severity.ToUpper()} - {updateTime}{Environment.NewLine}";
                    message += $"--- {update.Content}{Environment.NewLine}";
                }
            }
            testChannel.SendMessageAsync(message);
        }

        private List<UpdateDto> GetActiveUpdates(ServiceDto service)
        {
            List<UpdateDto> updateList = new List<UpdateDto>();
            foreach (IncidentDto incident in service.Incidents)
            {
                if (incident.Active && incident.Updates.Count > 0)
                {
                    updateList.AddRange(incident.Updates);
                }
            }
            return updateList;
        }
    }
}
