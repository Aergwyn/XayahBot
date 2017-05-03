#pragma warning disable 4014

using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Command.Remind;
using XayahBot.Database.DAO;
using XayahBot.Utility;

namespace XayahBot
{
    public class Program
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly RemindService _remindService;
        private readonly IDependencyMap _dependencyMap = new DependencyMap();

        private IgnoreDAO _ignoreDao = new IgnoreDAO();

        //

        public static void Main(string[] args)
        {
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        //

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
                await Logger.Error("No token supplied.");
            }
            await Task.Delay(2500);
        }

        private async Task InitializeAsync()
        {
            this._client.Ready += this.HandleReady;
            this._client.ChannelUpdated += this.HandleChannelUpdated;
            this._client.ChannelDestroyed += this.HandleChannelDestroyed;
            this._client.LeftGuild += this.HandleLeftGuild;
            this._client.MessageReceived += this.HandleMessageReceived;

            this._dependencyMap.Add(new IgnoreDAO());
            this._dependencyMap.Add(this._remindService);

            await this._commandService.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task StopBackgroundThreadsAsync()
        {
            await this._remindService.StopAsync();
        }

        //

        private Task HandleReady()
        {
            this.SetGame();
            this.StartBackgroundThreads();
            return Task.CompletedTask;
        }

        private void SetGame()
        {
            string game = string.IsNullOrWhiteSpace(Property.GameActive.Value) ? null : Property.GameActive.Value;
            this._client.SetGameAsync(game);
        }

        private Task StartBackgroundThreads()
        {
            this._remindService.StartAsync();
            //RiotStatusService.StartAsync(this._client); WiP
            return Task.CompletedTask;
        }

        private Task HandleChannelUpdated(SocketChannel preUpdateChannel, SocketChannel postUpdateChannel)
        {
            this._ignoreDao.UpdateAsync(preUpdateChannel.Id, ((IChannel)postUpdateChannel).Name);
            return Task.CompletedTask;
        }

        private Task HandleChannelDestroyed(SocketChannel deletedChannel)
        {
            this._ignoreDao.RemoveBySubjectIdAsync(deletedChannel.Id);
            return Task.CompletedTask;
        }

        private Task HandleLeftGuild(SocketGuild leftGuild)
        {
            this._ignoreDao.RemoveByGuildAsync(leftGuild.Id);
            return Task.CompletedTask;
        }

        private async Task HandleMessageReceived(SocketMessage arg)
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
                        IMessageChannel dmChannel = await context.User.CreateDMChannelAsync();
                        dmChannel?.SendMessageAsync($"This did not work! Reason: `{result.ErrorReason}`");
                    }
                    else if(this.IsInterestingError(result.Error))
                    {
                        Logger.Debug($"Command failed: {result.ErrorReason}");
                    }
                }
            }
        }

        private bool IsUserError(CommandError? error)
        {
            if (error == CommandError.UnmetPrecondition)
            {
                return true;
            }
            return false;
        }

        private bool IsInterestingError(CommandError? error)
        {
            if (error == CommandError.Exception || error == CommandError.ObjectNotFound || error  == CommandError.ParseFailed)
            {
                return true;
            }
            return false;
        }
    }
}
