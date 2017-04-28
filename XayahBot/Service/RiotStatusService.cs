using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.API.Controller;
using XayahBot.API.Model;
using XayahBot.API;

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
            }
            Task.Run(() => _instance.Start());
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
                while (this._running)
                {
                    // Interval in minutes
                    if (DateTime.UtcNow.Minute % 5 == 0)
                    {
                        if (!_checked)
                        {
                            LolStatusV3Controller controller = new LolStatusV3Controller();
                            AnalyzeData(await controller.Get(Region.EUW));
                            AnalyzeData(await controller.Get(Region.NA));
                            _checked = true;
                        }
                    }
                    else
                    {
                        _checked = false;
                    }
                    if (this._running)
                    {
                        await Task.Delay(20000); // Cycle every 20 seconds
                    }
                }
            }
        }

        private void AnalyzeData(ShardStatus status)
        {
            if (status != null && status.Services.Count > 0)
            {
                ISocketMessageChannel testChannel = this._client.GetChannel(301030133014200320) as ISocketMessageChannel;
                string message = string.Empty;
                message += $"__Status {status.Name}__{Environment.NewLine}";
                bool post = false;
                string subMsg = string.Empty;
                foreach (API.Model.Service srvc in status.Services)
                {
                    message += $"----{srvc.Name} | {srvc.Status}{Environment.NewLine}";
                    foreach (Incident inc in srvc.Incidents.Where(x => x.Updates.Count > 0))
                    {
                        subMsg += $"------{inc.Id} - {inc.Active} - {inc.Created_At}{Environment.NewLine}";
                        foreach (Message msg in inc.Updates.Where(x => x.Translations.Where(y => y.Locale.Equals("en_US")).ToList().Count > 0))
                        {
                            subMsg += $"--------{msg.Author} - {msg.Severity} - {msg.Created_At}{Environment.NewLine}";
                            foreach (Translation trsl in msg.Translations.Where(x => x.Locale.Equals("en_US")))
                            {
                                subMsg += $"Locale: {trsl.Locale}; Updated: {trsl.Updated_At}{Environment.NewLine}";
                                subMsg += $"Message: {trsl.Content}{Environment.NewLine}";
                                post = true;
                            }
                        }
                    }
                    if (post)
                    {
                        post = false;
                        message += subMsg;
                        subMsg = string.Empty;
                    }
                }
                testChannel.SendMessageAsync(message);
            }
        }
    }
}
