#pragma warning disable 4014

using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.Precondition;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Data
{
    [Group("data")]
    [Category(CategoryType.DATA)]
    public class CData : ModuleBase
    {
        [Command("champ"), Alias("c")]
        [Summary("Displays data of a specific champion. Name does not have to be exact.")]
        public async Task Champ([Remainder] string name)
        {
            IMessageChannel channel = await ChannelHelper.GetDMChannel(this.Context);
            DiscordFormatMessage message = await InfoService.GetChampionDataText(name);
            channel.SendMessageAsync(message.ToString());
        }
    }
}
