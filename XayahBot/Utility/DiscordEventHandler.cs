using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Command.Incidents;
using XayahBot.Command.Remind;
using XayahBot.Database.DAO;
using XayahBot.Extension;
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
            Task.Run(() => this.ProcessReady());
            return Task.CompletedTask;
        }

        private async Task ProcessReady()
        {
            try
            {
                DiscordSocketClient client = this._serviceProvider.GetService(typeof(DiscordSocketClient)) as DiscordSocketClient;
                RemindService remindService = this._serviceProvider.GetService(typeof(RemindService)) as RemindService;
                IncidentService incidentService = this._serviceProvider.GetService(typeof(IncidentService)) as IncidentService;
                string game = string.IsNullOrWhiteSpace(Property.GameActive.Value) ? null : Property.GameActive.Value;

                await client.SetGameAsync(game);
                await remindService.StartAsync();
                await incidentService.StartAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public Task HandleMessageReceived(SocketMessage arg)
        {
            Task.Run(() => this.ProcessMessageReceived(arg));
            return Task.CompletedTask;
        }

        private async Task ProcessMessageReceived(SocketMessage arg)
        {
            try
            {
                SocketUserMessage message = arg as SocketUserMessage;
                if (message == null || message.Author.IsBot)
                {
                    return;
                }
                DiscordSocketClient client = this._serviceProvider.GetService(typeof(DiscordSocketClient)) as DiscordSocketClient;
                CommandService commandService = this._serviceProvider.GetService(typeof(CommandService)) as CommandService;

                int pos = 0;
                CommandContext context = new CommandContext(client, message);
                if (context.IsPrivate || message.HasMentionPrefix(client.CurrentUser, ref pos))
                {
                    IResult result = await commandService.ExecuteAsync(context, pos, this._serviceProvider);
                    if (!result.IsSuccess)
                    {
                        if (this.IsUserError(result.Error))
                        {
                            IMessageChannel dmChannel = await ChannelProvider.GetDMChannelAsync(context);
                            FormattedEmbedBuilder errorResponse = new FormattedEmbedBuilder()
                                .AppendTitle($"{XayahReaction.Error} This didn't work")
                                .AddField("Why, you ask?", result.ErrorReason);
                            await dmChannel.SendEmbedAsync(errorResponse);
                        }
                        else if (this.IsInterestingError(result.Error))
                        {
                            Logger.Debug($"Command failed: {result.ErrorReason}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
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
