#pragma warning disable 4014

using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.Precondition;
using XayahBot.Database.DAO;
using XayahBot.Database.Model;
using XayahBot.Error;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Incidents
{
    [Group("incidents")]
    [Category(CategoryType.INCIDENT)]
    public class CIncidents : ModuleBase
    {
        private readonly IncidentSubscriberDAO _incidentSubscriberDao = new IncidentSubscriberDAO();
        private IncidentService _incidentService;

        public CIncidents(IncidentService incidentService)
        {
            this._incidentService = incidentService;
        }

        [Command("enable")]
        [RequireMod]
        [RequireContext(ContextType.Guild)]
        [Summary("Enables incident notifications for Riot's server in the mentioned channel.")]
        public async Task Enable(string channel)
        {
            ulong channelId = this.Context.Message.MentionedChannelIds.ElementAt(0);
            IChannel mentionedChannel = await this.Context.Guild.GetChannelAsync(channelId);
            await this.EnabledGuild(mentionedChannel);
            await this._incidentService.StartAsync();
        }

        private async Task EnabledGuild(IChannel channel)
        {
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .CreateFooter(this.Context);
            try
            {
                await this._incidentSubscriberDao.AddAsync(new TIncidentSubscriber
                {
                    ChannelId = channel.Id,
                    ChannelName = channel.Name,
                    GuildId = this.Context.Guild.Id
                });
                message.AppendDescription($"Enabled incident notifications for `{channel.Name}`.");
            }
            catch (AlreadyExistingException)
            {
                message.AppendDescription("Could not enable incident notifications because it is already enabled.");
            }
            catch (NotSavedException nsex)
            {
                Logger.Error($"Failed to add status setting for {this.Context.Guild.Name} ({this.Context.Guild.Id}).", nsex);
            }
        }

        [Command("disable")]
        [RequireMod]
        [RequireContext(ContextType.Guild)]
        [Summary("Disables incident notifications for Riot's server.")]
        public async Task Disable()
        {
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .CreateFooter(this.Context);
            try
            {
                await this._incidentSubscriberDao.RemoveAsync(this.Context.Guild.Id);
                message.AppendDescription("Disabled incident notifications.");
            }
            catch (NotExistingException)
            {
                message.AppendDescription("Could not disable incident notifications because it was not enabled.");
            }
            catch (NotSavedException nsex)
            {
                Logger.Error($"Failed to remove status setting from {this.Context.Guild.Name} ({this.Context.Guild.Id}).", nsex);
            }
            this.ReplyAsync("", false, message.ToEmbed());
        }
    }
}
