#pragma warning disable 4014

using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
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

        // ---

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly RemindService _remindService;
        private readonly IncidentService _incidentService;
        private readonly IDependencyMap _dependencyMap = new DependencyMap();

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
            this._remindService = RemindService.GetInstance(this._client);
            this._incidentService = IncidentService.GetInstance(this._client);
        }

        private async Task StartAsync()
        {
            this._client.Log += Logger.Log;
            await this.InitializeAsync();
            string token = FileReader.ReadFirstLine(Property.FilePath.Value + Property.FileToken.Value);
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
            this._dependencyMap.Add(this._client);
            this._dependencyMap.Add(this._remindService);
            this._dependencyMap.Add(this._incidentService);

            DiscordEventHandler eventHandler = new DiscordEventHandler(this._dependencyMap);
            this._client.Ready += eventHandler.HandleReady;
            this._client.ChannelDestroyed += eventHandler.HandleChannelDestroyed;
            this._client.LeftGuild += eventHandler.HandleLeftGuild;
            this._client.MessageReceived += this.HandleMessageReceived;

            this._commandService.AddTypeReader<RegionTypeReader>(new RegionTypeReader());
            await this._commandService.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task StopBackgroundThreadsAsync()
        {
            this._incidentService.Stop();
            await this._remindService.StopAsync();
        }

        public async Task HandleMessageReceived(SocketMessage arg)
        {
            int pos = 0;
            if (arg is SocketUserMessage message && (message.HasCharPrefix(char.Parse(Property.CmdPrefix.Value), ref pos) ||
                message.HasMentionPrefix(message.Discord.CurrentUser, ref pos)))
            {
                CommandContext context = new CommandContext(message.Discord, message);
                IResult result = await this._commandService.ExecuteAsync(context, pos, this._dependencyMap);
                if (!result.IsSuccess)
                {
                    if (this.IsUserError(result.Error))
                    {
                        IMessageChannel dmChannel = await ChannelRetriever.GetDMChannelAsync(context);
                        DiscordFormatEmbed errorResponse = new DiscordFormatEmbed();
                        errorResponse.AppendDescription("This did not work!")
                            .AppendDescription(Environment.NewLine)
                            .AppendDescription($"Reason: { result.ErrorReason}");
                        dmChannel.SendMessageAsync("", false, errorResponse.ToEmbed());
                    }
                    else if (this.IsInterestingError(result.Error))
                    {
                        Logger.Debug($"Command failed: {result.ErrorReason}");
                    }
                }
            }
        }

        private bool IsUserError(CommandError? error)
        {
            if (error == CommandError.UnmetPrecondition || error == CommandError.BadArgCount || error == CommandError.ParseFailed)
            {
                return true;
            }
            return false;
        }

        private bool IsInterestingError(CommandError? error)
        {
            if (error == CommandError.Exception || error == CommandError.ObjectNotFound)
            {
                return true;
            }
            return false;
        }
    }
}
