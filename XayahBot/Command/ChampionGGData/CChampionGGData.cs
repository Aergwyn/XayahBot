using System;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Command.Logic;
using XayahBot.Extension;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.ChampionGGData
{
    public class CChampionGGData : ToggleableModuleBase
    {
        protected override Property GetDisableProperty()
        {
            return Property.ChampionGGApiDisabled;
        }

        [Command("winrate")]
        public Task Winrate()
        {
            Task.Run(() => this.ProcessWinrate(LeagueRole.All));
            return Task.CompletedTask;
        }

        [Command("winrate")]
        public Task Winrate([OverrideTypeReader(typeof(LeagueRoleTypeReader))] LeagueRole role)
        {
            Task.Run(() => this.ProcessWinrate(role));
            return Task.CompletedTask;
        }

        private async Task ProcessWinrate(LeagueRole role)
        {
            try
            {
                FormattedEmbedBuilder message = await ChampionStatsBuilder.BuildForRoleAsync(role);
                message.CreateFooterIfNotDM(this.Context);
                await this.ReplyAsync(message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
