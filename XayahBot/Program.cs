using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using XayahBot.Command.Account;
using XayahBot.Command.Incidents;
using XayahBot.Command.Precondition;
using XayahBot.Command.Remind;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        // -

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;

        private IServiceProvider _serviceProvider;
        private readonly IServiceCollection _serviceCollection = new ServiceCollection();

        private Program()
        {
            this._client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });
            this._commandService = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
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
                    if (Console.ReadLine().ToLower().Equals("exit"))
                    {
                        exit = true;
                    }
                }

                await this._client.SetGameAsync(Property.GameShutdown.Value);
                await this.StopBackgroundThreads(this._serviceProvider);
                await this._client.StopAsync();
            }
            else
            {
                await Logger.Error("No token supplied.");
            }
            await Task.Delay(2500);
        }

        private async Task InitializeAsync()
        {
            this._serviceCollection.AddSingleton(this._client);
            this._serviceCollection.AddSingleton(this._commandService);
            this._serviceCollection.AddSingleton(RemindService.GetInstance(this._client));
            this._serviceCollection.AddSingleton(IncidentService.GetInstance(this._client));
            this._serviceCollection.AddSingleton(RegistrationService.GetInstance(this._client));

            this._serviceProvider = this._serviceCollection.BuildServiceProvider(true);

            await this._commandService.AddModulesAsync(Assembly.GetEntryAssembly());

            DiscordEventHandler eventHandler = new DiscordEventHandler(this._serviceProvider);
            this._client.Log += Logger.Log;
            this._commandService.Log += Logger.Log;
            this._client.Ready += eventHandler.HandleReady;
            this._client.ChannelDestroyed += eventHandler.HandleChannelDestroyed;
            this._client.LeftGuild += eventHandler.HandleLeftGuild;
            this._client.MessageReceived += eventHandler.HandleMessageReceived;

            this._commandService.AddTypeReader<RegionTypeReader>(new RegionTypeReader());
        }

        private async Task StopBackgroundThreads(IServiceProvider serviceProvider)
        {
            await (serviceProvider.GetService(typeof(RemindService)) as RemindService)?.StopAsync();
            await (serviceProvider.GetService(typeof(IncidentService)) as IncidentService)?.StopAsync();
            await (serviceProvider.GetService(typeof(RegistrationService)) as RegistrationService)?.StopAsync();
        }
    }
}
