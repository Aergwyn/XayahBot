using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Utility;
using XayahBot.Service;

namespace XayahBot
{
    public class Program
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IDependencyMap _dependencyMap = new DependencyMap();

        //

        public static void Main(string[] args)
        {
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        //

        private Program()
        {
            this._commandService = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            });
            this._client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });
        }

#pragma warning disable 4014 // Intentional
        private async Task StartAsync()
        {
            this._client.Log += Logger.Log;
            this._client.Ready += this.HandleReady;
            //this._client.ChannelUpdated
            //this._client.ChannelDestroyed
            //this._client.LeftGuild

            await InitCommandsAsync();

            string token = File.ReadLines(Property.FilePath.Value + Property.FileToken.Value).ElementAt(0); // I'm not gonna show it
            if (!string.IsNullOrWhiteSpace(token))
            {
                await this._client.LoginAsync(TokenType.Bot, token);
                await this._client.StartAsync();
                //
                bool exit = false;
                while (!exit)
                {
                    if (Console.ReadLine().ToLower().Equals("exit"))
                    {
                        exit = true;
                    }
                }
                //
                await this._client.SetGameAsync(Property.GameShutdown.Value);
                await this._client.StopAsync();
            }
            else
            {
                Logger.Log(new LogMessage(LogSeverity.Error, nameof(Program), "No token supplied."));
            }
            await Task.Delay(2500); // Wait a bit
        }
#pragma warning restore 4014

        private async Task InitCommandsAsync()
        {
            // EventHandler
            this._client.MessageReceived += this.HandleCommand;
            // Map
            this._dependencyMap.Add(this._client);
            // Command
            await this._commandService.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        //

        private Task HandleReady()
        {
            string game = Property.GameActive.Value;
            if (!string.IsNullOrWhiteSpace(game))
            {
                this._client.SetGameAsync(game);
            }
            else
            {
                this._client.SetGameAsync(null); // May not be needed but if bot restarts you overwrite status at least
            }
            //RiotStatusService.StartAsync(this._client);
            return Task.CompletedTask;
        }

#pragma warning disable 4014 // Intentional
        private async Task HandleCommand(SocketMessage arg)
        {
            int pos = 0;
            if (arg is SocketUserMessage message && (message.HasCharPrefix(char.Parse(Property.CmdPrefix.Value), ref pos) || message.HasMentionPrefix(message.Discord.CurrentUser, ref pos)))
            {
                CommandContext context = new CommandContext(message.Discord, message);
                IResult result = await this._commandService.ExecuteAsync(context, pos, this._dependencyMap);
                if (!result.IsSuccess)
                {
                    if (result.ErrorReason.Contains("Invalid context for command") ||
                        result.ErrorReason.Contains("permission (for this bot) to execute") ||
                        result.ErrorReason.Contains("on the ignore list for this bot"))
                    {
                        IMessageChannel dmChannel = await context.User.CreateDMChannelAsync();
                        if (dmChannel != null)
                        {
                            dmChannel.SendMessageAsync($"This did not work! Reason: `{result.ErrorReason}`");
                        }
                    }
                    else if(result.Error != CommandError.UnknownCommand)
                    {
                        Logger.Log(LogSeverity.Debug, nameof(Program), $"Command failed: {result.ErrorReason}");
                    }
                   // Mostly for debugging
                   //await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }
#pragma warning restore 4014
    }
}
