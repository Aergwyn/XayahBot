using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.Logic;
using XayahBot.Database.DAO;
using XayahBot.Database.Model;
using XayahBot.Error;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Incidents
{
    [Group("incidents")]
    public class CIncidents : ToggleableModuleBase
    {
        private readonly IncidentService _incidentService;
        private readonly IncidentSubscriberDAO _incidentSubscriberDAO = new IncidentSubscriberDAO();

        public CIncidents(IncidentService incidentService)
        {
            this._incidentService = incidentService;
        }

        protected override Property GetDisableProperty()
        {
            return Property.IncidentDisabled;
        }

        [Command("on")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        public Task Enable(string channel)
        {
            Task.Run(() => this.EnableIncidents(channel));
            return Task.CompletedTask;
        }

        private async Task EnableIncidents(string channel)
        {
            if (this.IsDisabled())
            {
                this.NotifyDisabledCommand();
                return;
            }
            ulong channelId = this.Context.Message.MentionedChannelIds.First();
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .CreateFooter(this.Context);
            try
            {
                IChannel mentionedChannel = await this.Context.Guild.GetChannelAsync(channelId) ?? throw new NoChannelException();
                TIncidentSubscriber subscriber = null;
                try
                {
                    subscriber = this._incidentSubscriberDAO.GetByGuildId(this.Context.Guild.Id);
                }
                catch (NotExistingException)
                {
                    subscriber = new TIncidentSubscriber
                    {
                        GuildId = this.Context.Guild.Id
                    };
                }
                subscriber.ChannelId = mentionedChannel.Id;
                await this._incidentSubscriberDAO.SaveAsync(subscriber);
                message
                    .AppendTitle($"{XayahReaction.Success} Done")
                    .AppendDescription($"Incident notifications will now go to `{mentionedChannel.Name}`.");
            }
            catch (NoChannelException)
            {
                message
                    .AppendTitle($"{XayahReaction.Error} This didn't work")
                    .AppendDescription($"I couldn't find the mentioned channel.");
            }
            await this.ReplyAsync("", false, message.ToEmbed());
            await this._incidentService.StartAsync();
        }

        [Command("status")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        public Task Status()
        {
            Task.Run(() => this.ShowStatus());
            return Task.CompletedTask;
        }

        private async Task ShowStatus()
        {
            if (this.IsDisabled())
            {
                this.NotifyDisabledCommand();
                return;
            }
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .CreateFooter(this.Context);
            try
            {
                TIncidentSubscriber subscriber = this._incidentSubscriberDAO.GetByGuildId(this.Context.Guild.Id);
                IChannel channel = await this.Context.Guild.GetChannelAsync(subscriber.ChannelId) ?? throw new NoChannelException();
                message
                    .AppendTitle($"{XayahReaction.Success} Enabled")
                    .AppendDescription($"Incident notifications are currently enabled and posted in `{channel.Name}`.");
            }
            catch (NotExistingException)
            {
                message
                    .AppendTitle($"{XayahReaction.Error} Disabled")
                    .AppendDescription("Incident notifications are currently disabled.");
            }
            catch (NoChannelException)
            {
                message
                    .AppendTitle($"{XayahReaction.Error} This didn't work")
                    .AppendDescription($"I couldn't find the configured channel.");
            }
            await this.ReplyAsync("", false, message.ToEmbed());
        }

        [Command("off")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        public Task Disable()
        {
            Task.Run(() => this.DisableIncidents());
            return Task.CompletedTask;
        }

        private async Task DisableIncidents()
        {
            if (this.IsDisabled())
            {
                this.NotifyDisabledCommand();
                return;
            }
            await this._incidentSubscriberDAO.RemoveByGuildIdAsync(this.Context.Guild.Id);
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .CreateFooter(this.Context)
                .AppendTitle($"{XayahReaction.Success} Done");
                message.AppendDescription("Incident notifications are now disabled.");
            await this.ReplyAsync("", false, message.ToEmbed());
        }
    }
}
