using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.API.Error;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Data
{
    public class CData : ModuleBase
    {
        [Command("champ")]
        public Task Data([Remainder] string name)
        {
            Task.Run(() => this.PostChampion(name));
            return Task.CompletedTask;
        }

        private async Task PostChampion(string name)
        {
            IMessageChannel channel = await ChannelProvider.GetDMChannelAsync(this.Context);
            DiscordFormatEmbed message = null;
            try
            {
                message = await ChampionDataBuilder.BuildAsync(name);
            }
            catch (ErrorResponseException ex)
            {
                message = new DiscordFormatEmbed()
                    .AppendTitle($"{XayahReaction.Error} This didn't work")
                    .AppendDescription("Apparently the Riot-API refuses cooperation. Have some patience while I convince them again...");
                Logger.Error($"The StaticData-API returned an error.", ex);
            }
            if (!(this.Context as CommandContext).IsPrivate)
            {
                message.CreateFooter(this.Context);
            }
            await channel.SendMessageAsync("", false, message.ToEmbed());
        }
    }
}
