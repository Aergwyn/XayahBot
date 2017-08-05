using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Database.DAO;
using XayahBot.Database.Model;
using XayahBot.Error;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Incidents
{
    [Group("incidents")]
    public class CIncidents : ModuleBase
    {
        private readonly DiscordSocketClient _client;
        private readonly IncidentService _incidentService;
        private readonly IncidentSubscriberDAO _incidentSubscriberDao = new IncidentSubscriberDAO();

        public CIncidents(IServiceProvider serviceProvider)
        {
            this._client = serviceProvider.GetService(typeof(DiscordSocketClient)) as DiscordSocketClient;
            this._incidentService = serviceProvider.GetService(typeof(IncidentService)) as IncidentService;
        }

        [Command("enable")]
        [RequireOwner]
        [RequireContext(ContextType.Guild)]
        [Summary("Enables incident notifications from Riot in the mentioned channel.")]
        public async Task Enable(string channel)
        {
            ulong channelId = this.Context.Message.MentionedChannelIds.First();
            IChannel mentionedChannel = await this.Context.Guild.GetChannelAsync(channelId) ?? throw new NoChannelException();
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .CreateFooter(this.Context);
            try
            {
                await this._incidentSubscriberDao.SaveAsync(new TIncidentSubscriber
                {
                    ChannelId = mentionedChannel.Id,
                    GuildId = this.Context.Guild.Id
                });
                message.AppendDescription($"Enabled incident notifications in `{mentionedChannel.Name}`.");
            }
            catch (AlreadyExistingException)
            {
                message.AppendDescription("Could not enable incident notifications because it is already enabled.");
            }
            await this.ReplyAsync("", false, message.ToEmbed());
            await this._incidentService.StartAsync();
        }

        [Command("status")]
        [RequireOwner]
        [RequireContext(ContextType.Guild)]
        [Summary("Shows if incident notifications are enabled or disabled for this server.")]
        public async Task Status()
        {
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .CreateFooter(this.Context);
            try
            {
                TIncidentSubscriber subscriber = this._incidentSubscriberDao.GetSingleByGuildId(this.Context.Guild.Id);
                IChannel channel = await this.Context.Guild.GetChannelAsync(subscriber.ChannelId) as IChannel;
                message.AppendDescription($"Incident notifications are enabled and will be posted in `{channel.Name}`.");
            }
            catch (NotExistingException)
            {
                message.AppendDescription("Incident notifications are currently disabled.");
            }
            this.ReplyAsync("", false, message.ToEmbed());
        }

        [Command("disable")]
        [RequireOwner]
        [RequireContext(ContextType.Guild)]
        [Summary("Disables incident notifications from Riot.")]
        public async Task Disable()
        {
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .CreateFooter(this.Context);
            try
            {
                await this._incidentSubscriberDao.RemoveByGuildIdAsync(this.Context.Guild.Id);
                message.AppendDescription("Disabled incident notifications.");
            }
            catch (NotExistingException)
            {
                message.AppendDescription("Could not disable incident notifications because it was not enabled.");
            }
            this.ReplyAsync("", false, message.ToEmbed());
        }
    }
}
