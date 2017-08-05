#pragma warning disable 4014

using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Command.Incidents;
using XayahBot.Command.Remind;
using XayahBot.Database.DAO;
using XayahBot.Utility.Messages;

namespace XayahBot.Utility
{
    public class DiscordEventHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IncidentSubscriberDAO _incidentSubscriberDao = new IncidentSubscriberDAO();

        public DiscordEventHandler(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public Task HandleReady()
        {
            DiscordSocketClient client = this._serviceProvider.GetService(typeof(DiscordSocketClient)) as DiscordSocketClient;
            RemindService remindService = this._serviceProvider.GetService(typeof(RemindService)) as RemindService;
            IncidentService incidentService = this._serviceProvider.GetService(typeof(IncidentService)) as IncidentService;

            string game = string.IsNullOrWhiteSpace(Property.GameActive.Value) ? null : Property.GameActive.Value;
            client.SetGameAsync(game);
            remindService.StartAsync();
            incidentService.StartAsync();

            return Task.CompletedTask;
        }

        public async Task HandleMessageReceived(SocketMessage arg)
        {
            SocketUserMessage message = arg as SocketUserMessage;
            if (message == null)
            {
                return;
            }
            int pos = 0;
            DiscordSocketClient client = this._serviceProvider.GetService(typeof(DiscordSocketClient)) as DiscordSocketClient;
            if (message.HasMentionPrefix(client.CurrentUser, ref pos))
            {
                CommandContext context = new CommandContext(client, message);
                CommandService commandService = this._serviceProvider.GetService(typeof(CommandService)) as CommandService;
                IResult result = await commandService.ExecuteAsync(context, pos, this._serviceProvider);

                if (!result.IsSuccess)
                {
                    if (this.IsUserError(result.Error))
                    {
                        IMessageChannel dmChannel = await ChannelProvider.GetDMChannelAsync(context);
                        DiscordFormatEmbed errorResponse = new DiscordFormatEmbed()
                            .AppendTitle($"{XayahReaction.Error} This didn't work")
                            .AddField("Why, you ask?", result.ErrorReason);
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

        public Task HandleChannelDestroyed(SocketChannel deletedChannel)
        {
            this._incidentSubscriberDao.RemoveByChannelIdAsync(deletedChannel.Id);
            return Task.CompletedTask;
        }

        public Task HandleLeftGuild(SocketGuild leftGuild)
        {
            this._incidentSubscriberDao.RemoveByGuildIdAsync(leftGuild.Id);
            return Task.CompletedTask;
        }
    }
}
