using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.Logic;
using XayahBot.Extension;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.RiotData
{
    public class CRiotData : ToggleableModuleBase
    {
        protected override Property GetDisableProperty()
        {
            return Property.RiotApiDisabled;
        }

        [Command("champ")]
        public Task Champ([Remainder] string name)
        {
            Task.Run(() => this.ProcessChamp(name));
            return Task.CompletedTask;
        }

        private async Task ProcessChamp(string name)
        {
            try
            {
                if (this.IsDisabled())
                {
                    this.NotifyDisabledCommand();
                    return;
                }
                IMessageChannel channel = await ChannelProvider.GetDMChannelAsync(this.Context);
                FormattedEmbedBuilder message = await ChampionDataBuilder.BuildAsync(name);
                await channel.SendEmbedAsync(message);
                await this.Context.Message.AddReactionIfNotDMAsync(this.Context, XayahReaction.Envelope);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        [Command("item")]
        public Task Item([Remainder] string name)
        {
            Task.Run(() => this.ProcessItem(name));
            return Task.CompletedTask;
        }

        private async Task ProcessItem(string name)
        {
            try
            {
                if (this.IsDisabled())
                {
                    this.NotifyDisabledCommand();
                    return;
                }
                IMessageChannel channel = await ChannelProvider.GetDMChannelAsync(this.Context);
                FormattedEmbedBuilder message = await ItemDataBuilder.BuildAsync(name);
                await channel.SendEmbedAsync(message);
                await this.Context.Message.AddReactionIfNotDMAsync(this.Context, XayahReaction.Envelope);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
