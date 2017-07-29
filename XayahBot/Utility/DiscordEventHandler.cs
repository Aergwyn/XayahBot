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
        private readonly IgnoreListDAO _ignoreListDao = new IgnoreListDAO();
        private readonly IncidentSubscriberDAO _incidentSubscriberDao = new IncidentSubscriberDAO();

        public DiscordEventHandler(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public async Task HandleReady()
        {
            DiscordSocketClient client = this._serviceProvider.GetService(typeof(DiscordSocketClient)) as DiscordSocketClient;
            string game = string.IsNullOrWhiteSpace(Property.GameActive.Value) ? null : Property.GameActive.Value;
            await client.SetGameAsync(game);

            RemindService remindService = this._serviceProvider.GetService(typeof(RemindService)) as RemindService;
            IncidentService incidentService = this._serviceProvider.GetService(typeof(IncidentService)) as IncidentService;
            await remindService.StartAsync();
            await incidentService.StartAsync();
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
            if ((message.HasCharPrefix(char.Parse(Property.CmdPrefix.Value), ref pos) || message.HasMentionPrefix(client.CurrentUser, ref pos)))
            {
                CommandContext context = new CommandContext(client, message);
                CommandService commandService = this._serviceProvider.GetService(typeof(CommandService)) as CommandService;
                IResult result = await commandService.ExecuteAsync(context, pos, this._serviceProvider);

                if (!result.IsSuccess)
                {
                    if (this.IsUserError(result.Error))
                    {
                        IMessageChannel dmChannel = await ChannelProvider.GetDMChannelAsync(context);

                        DiscordFormatEmbed errorResponse = new DiscordFormatEmbed();
                        errorResponse.AppendDescription("This did not work!")
                            .AppendDescription(Environment.NewLine)
                            .AppendDescription($"Reason: { result.ErrorReason}");

                        await dmChannel.SendMessageAsync("", false, errorResponse.ToEmbed());
                    }
                    else if (this.IsInterestingError(result.Error))
                    {
                        await Logger.Debug($"Command failed: {result.ErrorReason}");
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

        public async Task HandleChannelDestroyed(SocketChannel deletedChannel)
        {
            if (this._ignoreListDao.HasSubject(deletedChannel.Id))
            {
                await this._ignoreListDao.RemoveBySubjectIdAsync(deletedChannel.Id);
            }
            if (this._incidentSubscriberDao.HasChannelSubscribed(deletedChannel.Id))
            {
                await this._incidentSubscriberDao.RemoveByChannelIdAsync(deletedChannel.Id);
            }
        }

        public async Task HandleLeftGuild(SocketGuild leftGuild)
        {
            await this._ignoreListDao.RemoveByGuildIdAsync(leftGuild.Id);
            if (this._incidentSubscriberDao.HasGuildSubscribed(leftGuild.Id))
            {
                await this._incidentSubscriberDao.RemoveByGuildIdAsync(leftGuild.Id);
            }
        }
    }
}
