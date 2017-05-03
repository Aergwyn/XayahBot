#pragma warning disable 4014

using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Utility;

namespace XayahBot.Command.Data
{
    [Group("data")]
    public class CData : ModuleBase
    {
        [Command("champ"), Alias("c")]
        [Summary("Displays data of a specific champion. Name does not have to be exact.")]
        public async Task Champ([Remainder] string name)
        {
            IMessageChannel channel = await ResponseHelper.GetDMChannel(this.Context);
            DiscordFormatMessage message = await InfoService.GetChampionDataText(name);
            channel.SendMessageAsync(message.ToString());
        }
    }
}
