﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Utility;

namespace XayahBot
{
    public class Program
    {
        private readonly DiscordSocketClient _client;

        private readonly IDependencyMap _map = new DependencyMap();
        private readonly CommandService _commands = new CommandService();

        //

        public static void Main(string[] args)
        {
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        //

        private Program()
        {
            // Pre Start Stuff
            string path = Property.FilePath.Value;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //
            this._client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
            });
        }

#pragma warning disable 4014 // Intentional
        private async Task StartAsync()
        {
            this._client.Log += Logger.Log;
            this._client.Ready += this.HandleReady;

            await InitCommandsAsync();

            string token = string.Empty; // I'm not gonna show it here
            using (StreamReader reader = new StreamReader(new FileStream(Property.FilePath.Value + Property.FileToken.Value, FileMode.Open)))
            {
                token = reader.ReadLine();
            }
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
            this._map.Add(this._client);
            // Command
            await this._commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        //

        private Task HandleReady()
        {
            string game = Property.GameActive.Value;
            if (!string.IsNullOrWhiteSpace(game))
            {
                this._client.SetGameAsync(Property.GameActive.Value);
            }
            else
            {
                this._client.SetGameAsync(null); // May not be needed but if bot restarts you overwrite status at least
            }
            return Task.CompletedTask;
        }

#pragma warning disable 4014 // Intentional
        private async Task HandleCommand(SocketMessage arg)
        {
            SocketUserMessage message = arg as SocketUserMessage;
            if (message == null)
            {
                return;
            }
            int pos = 0;
            if (message.HasCharPrefix(char.Parse(Property.CmdPrefix.Value), ref pos) || message.HasMentionPrefix(message.Discord.CurrentUser, ref pos))
            {
                CommandContext context = new CommandContext(message.Discord, message);
                IResult result = await this._commands.ExecuteAsync(context, pos, this._map);
                if (!result.IsSuccess)
                {
                    if (result.ErrorReason.Contains("Invalid context for command"))
                    {
                        IMessageChannel dmChannel = await context.User.CreateDMChannelAsync();
                        if (dmChannel != null)
                        {
                            dmChannel.SendMessageAsync($"This did not work. Reason: `{result.ErrorReason}`");
                        }
                    }
                    else
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