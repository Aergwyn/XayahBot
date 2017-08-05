using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using XayahBot.Command.Incidents;
using XayahBot.Command.Logic;
using XayahBot.Command.Remind;
using XayahBot.Database;
using XayahBot.Utility;

namespace XayahBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        // ---

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService = new CommandService();

        private IServiceProvider _serviceProvider;
        private readonly IServiceCollection _serviceCollection = new ServiceCollection();

        private Program()
        {
            this._client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });
        }

        private async Task StartAsync()
        {
            await this.InitializeAsync();
            string token = FileReader.GetFirstLine(Property.FilePath.Value + Property.FileToken.Value);
            if (!string.IsNullOrWhiteSpace(token))
            {
                await this._client.LoginAsync(TokenType.Bot, token);
                await this._client.StartAsync();

                bool exit = false;
                while (!exit)
                {
                    if (Console.ReadLine().ToLower().Equals("logout"))
                    {
                        exit = true;
                    }
                }

                await this._client.SetGameAsync("shutting down...");
                await this.StopBackgroundThreadsAsync();
                await this._client.StopAsync();
            }
            else
            {
                Logger.Error("No token supplied.");
            }
            await Task.Delay(2500);
        }

        private async Task InitializeAsync()
        {
            this._serviceCollection.AddSingleton(this._client);
            this._serviceCollection.AddSingleton(this._commandService);
            this._serviceCollection.AddSingleton(RemindService.GetInstance(this._client));
            this._serviceCollection.AddSingleton(IncidentService.GetInstance(this._client));

            this._serviceProvider = this._serviceCollection.BuildServiceProvider(true);

            await this._commandService.AddModulesAsync(Assembly.GetEntryAssembly());
            this._commandService.AddTypeReader<TimeUnitTypeReader>(new TimeUnitTypeReader());

            DiscordEventHandler eventHandler = new DiscordEventHandler(this._serviceProvider);
            this._client.Log += Logger.Log;
            this._commandService.Log += Logger.Log;
            this._client.Ready += eventHandler.HandleReady;
            this._client.ChannelDestroyed += eventHandler.HandleChannelDestroyed;
            this._client.LeftGuild += eventHandler.HandleLeftGuild;
            this._client.MessageReceived += eventHandler.HandleMessageReceived;

            using (GeneralContext database = new GeneralContext())
            {
                await database.Database.EnsureCreatedAsync();
            }
        }

        private async Task StopBackgroundThreadsAsync()
        {
            await (this._serviceProvider.GetService(typeof(RemindService)) as RemindService)?.StopAsync();
            await (this._serviceProvider.GetService(typeof(IncidentService)) as IncidentService)?.StopAsync();
        }
    }
}
