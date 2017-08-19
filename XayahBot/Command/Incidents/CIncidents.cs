using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.Logic;
using XayahBot.Database.DAO;
using XayahBot.Database.Model;
using XayahBot.Error;
using XayahBot.Extension;
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
            return Property.RiotApiDisabled;
        }

        [Command("on")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        public Task Enable(string channel)
        {
            Task.Run(() => this.ProcessEnable(channel));
            return Task.CompletedTask;
        }

        private async Task ProcessEnable(string channel)
        {
            try
            {
                if (this.IsDisabled())
                {
                    this.NotifyDisabledCommand();
                    return;
                }
                FormattedEmbedBuilder message = new FormattedEmbedBuilder()
                    .CreateFooterIfNotDM(this.Context);
                try
                {
                    IReadOnlyCollection<IGuildChannel> guildChannel = await this.Context.Guild.GetChannelsAsync();
                    if (this.Context.Message.MentionedChannelIds.Count() == 0)
                    {
                        throw new NoChannelException();
                    }
                    IGuildChannel validChannel = guildChannel.FirstOrDefault(x => x.Id.Equals(this.Context.Message.MentionedChannelIds.First())) ?? throw new NoChannelException();
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
                    subscriber.ChannelId = validChannel.Id;
                    await this._incidentSubscriberDAO.SaveAsync(subscriber);

                    message.AppendTitle($"{XayahReaction.Success} Done")
                        .AppendDescription($"Incident notifications will now go to `{validChannel.Name}`.");
                    await this._incidentService.StartAsync();
                }
                catch (NoChannelException)
                {
                    message
                        .AppendTitle($"{XayahReaction.Error} This didn't work")
                        .AppendDescription($"I couldn't find the channel on this server.");
                }
                await this.ReplyAsync(message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        [Command("status")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        public Task Status()
        {
            Task.Run(() => this.ProcessStatus());
            return Task.CompletedTask;
        }

        private async Task ProcessStatus()
        {
            try
            {
                if (this.IsDisabled())
                {
                    this.NotifyDisabledCommand();
                    return;
                }
                FormattedEmbedBuilder message = new FormattedEmbedBuilder()
                    .CreateFooterIfNotDM(this.Context);
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
                    await this._incidentSubscriberDAO.RemoveByGuildIdAsync(this.Context.Guild.Id);
                    message
                        .AppendTitle($"{XayahReaction.Error} This didn't work")
                        .AppendDescription($"I couldn't find the configured channel and thus automatically disabled incident notifications on this server.");
                }
                await this.ReplyAsync(message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        [Command("off")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        public Task Disable()
        {
            Task.Run(() => this.ProcessOff());
            return Task.CompletedTask;
        }

        private async Task ProcessOff()
        {
            try
            {
                if (this.IsDisabled())
                {
                    this.NotifyDisabledCommand();
                    return;
                }
                await this._incidentSubscriberDAO.RemoveByGuildIdAsync(this.Context.Guild.Id);
                FormattedEmbedBuilder message = new FormattedEmbedBuilder()
                    .CreateFooterIfNotDM(this.Context)
                    .AppendTitle($"{XayahReaction.Success} Done");
                message.AppendDescription("Incident notifications are now disabled.");
                await this.ReplyAsync(message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
