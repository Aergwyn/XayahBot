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

        //[Command("winrate")]
        public Task Winrate([OverrideTypeReader(typeof(LeagueRoleTypeReader))] LeagueRole role = null)
        {
            Task.Run(() => this.ProcessWinrate(role));
            return Task.CompletedTask;
        }

        private async Task ProcessWinrate(LeagueRole role)
        {
            try
            {
                if (role == null)
                {
                    role = LeagueRole.All;
                }
                ChampionStatsBuilder statsBuilder = new ChampionStatsBuilder();
                FormattedEmbedBuilder message = await statsBuilder.BuildForRoleAsync(role);
                message.CreateFooterIfNotDM(this.Context);
                await this.ReplyAsync(message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        //[Command("stats")]
        public Task Stats([OverrideTypeReader(typeof(LeagueRoleTypeReader))] LeagueRole role, [Remainder] string name)
        {
            //Task.Run(() => this.ProcessStats());
            return Task.CompletedTask;
        }

        //[Command("stats")]
        public Task Stats([Remainder] string name)
        {
            //Task.Run(() => this.ProcessStats());
            return Task.CompletedTask;
        }

        //private async Task ProcessStats()
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex);
        //    }
        //}
    }
}
