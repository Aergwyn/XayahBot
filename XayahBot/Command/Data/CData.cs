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
        [Command("champ")]
        public Task Data([Remainder] string name)
        {
            Task.Run(() => this.PostChampion(name));
            return Task.CompletedTask;
        }

        protected override Property GetDisableProperty()
        {
            return Property.ChampDisabled;
        }

        private async Task PostChampion(string name)
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
            if (!(this.Context as CommandContext).IsPrivate)
            {
                message.CreateFooter(this.Context);
            }
            await channel.SendEmbedAsync(message);
        }
    }
}
