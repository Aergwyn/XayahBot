using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.Precondition;
using XayahBot.Utility;

namespace XayahBot.Command.Data
{
    [Group("data")]
    [Category(CategoryType.DATA)]
    public class CData : ModuleBase
    {
        [Group("champ"), Alias("c")]
        public class CChamp : ModuleBase
        {
            [Command("misc")]
            [Summary("Displays misc data of a champion.")]
            public async Task Misc([Remainder] string name)
            {
                await this.BuildAndPost(ChampionDataType.MISC, name);
            }

            [Command("spells")]
            [Summary("Displays spells of a champion.")]
            public async Task Spell([Remainder] string name)
            {
                await this.BuildAndPost(ChampionDataType.SPELLS, name);
            }

            [Command("stats")]
            [Summary("Displays stats (with stat growth) of a champion.")]
            public async Task Stats([Remainder] string name)
            {
                await this.BuildAndPost(ChampionDataType.STATS, name);
            }

            private async Task BuildAndPost(ChampionDataType type, string name)
            {
                IMessageChannel channel = await ChannelRetriever.GetDMChannel(this.Context);
                ChampionDataBuilder builder = new ChampionDataBuilder(channel, name);
                await builder.BuildAsync(type);
                await builder.PostAsync();
            }
        }
    }
}
