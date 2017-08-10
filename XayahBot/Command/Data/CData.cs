using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.API.Error;
using XayahBot.Command.Logic;
using XayahBot.Extension;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Data
{
    public class CData : ToggleableModuleBase
    {
        protected override Property GetDisableProperty()
        {
            return Property.ChampDisabled;
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
                FormattedEmbedBuilder message = null;
                try
                {
                    message = await ChampionDataBuilder.BuildAsync(name);
                }
                catch (ErrorResponseException ex)
                {
                    message = new FormattedEmbedBuilder()
                        .AppendTitle($"{XayahReaction.Error} This didn't work")
                        .AppendDescription("Apparently the Riot-API refuses cooperation. Have some patience while I convince them again...");
                    Logger.Error($"The StaticData-API returned an error.", ex);
                }
                message.CreateFooterIfNotDM(this.Context);
                await channel.SendEmbedAsync(message);
                await this.Context.Message.AddReactionIfNotDMAsync(this.Context, XayahReaction.Success);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
