using System;
using System.Globalization;
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
                //RiotStatusApi statusNa = new RiotStatusApi(Region.NA);
                while (this._running)
                {
                    if (DateTime.UtcNow.Minute % 5 == 0)
                    {
                        if (!_checked)
                        {
                            AnalyzeData(await statusEuw.GetStatusAsync());
                            //AnalyzeData(await statusNa.GetStatusAsync());
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
            if (status != null && status.Services.Count > 0)
            {
                ISocketMessageChannel testChannel = this._client.GetChannel(301030133014200320) as ISocketMessageChannel;
                string message = string.Empty;
                message += $"__Status {status.Name}__{Environment.NewLine}";
                string subMsg = string.Empty;
                foreach (ServiceDto service in status.Services)
                {
                    message += $"{service.Name} | {service.Status}{Environment.NewLine}";
                    foreach (IncidentDto incident in service.Incidents.Where(x => x.Active && x.Updates.Count > 0))
                    {
                        for (int i = 0; i < incident.Updates.Count; i++)
                        {
                            if (i > 0)
                            {
                                message += Environment.NewLine;
                            }
                            UpdateDto update = incident.Updates.ElementAt(i);
                            DateTime.TryParse(update.UpdateTime, out DateTime updateTime);
                            message += $"---- {update.Severity.ToUpper()} - {updateTime}{Environment.NewLine}";
                            message += $"---- {update.Content}{Environment.NewLine}";
                        }
                    }
                }
                testChannel.SendMessageAsync(message);
            }
        }
    }
}
