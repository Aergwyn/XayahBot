using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XayahBot.API.ChampionGG;
using XayahBot.API.ChampionGG.Model;
using XayahBot.API.Riot;
using XayahBot.API.Riot.Model;
using XayahBot.Error;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.ChampionGGData
{
    public class ChampionStatsBuilder
    {
        public static async Task<FormattedEmbedBuilder> BuildForRoleAsync(LeagueRole role)
        {
            ChampionStatsBuilder statsBuilder = new ChampionStatsBuilder();
            FormattedEmbedBuilder message = new FormattedEmbedBuilder();
            try
            {
                List<ChampionStatsDto> championStats = await statsBuilder.GetChampionGGStatsAsync();
                championStats.Sort((a, b) => b.WinRate.CompareTo(a.WinRate));
                List<ChampionDto> championList = await statsBuilder.GetRiotChampionListAsync();

                if (LeagueRole.All.Equals(role))
                {
                    ChampionStatsDto first = championStats.ElementAtOrDefault(0);
                    message.AppendTitle($"{XayahReaction.Clipboard} Winrates for all roles");
                    if (first != null)
                    {
                        message.AppendTitle($" (Patch {first.Patch})");
                    }
                    statsBuilder.AppendHighestAndLowestPerRole(championStats, championList, message);
                }
                else
                {
                    // TODO
                }
            }
            catch (NoApiResultException)
            {
                message = new FormattedEmbedBuilder()
                    .AppendTitle($"{XayahReaction.Error} This didn't work")
                    .AppendDescription("Apparently some random API refuses cooperation. Have some patience while I convince them again...");
            }
            return message;
        }

        // ---

        private readonly ChampionGGChampions _championGGChampions = new ChampionGGChampions();
        private readonly RiotStaticData _riotStaticData = new RiotStaticData(Region.EUW);

        private ChampionStatsBuilder()
        {
        }

        private async Task<List<ChampionStatsDto>> GetChampionGGStatsAsync()
        {
            return await this._championGGChampions.GetChampionsAsync();
        }

        private async Task<List<ChampionDto>> GetRiotChampionListAsync()
        {
            ChampionListDto championList = await this._riotStaticData.GetChampionsAsync();
            return championList.Data.Values.ToList();
        }

        private void AppendHighestAndLowestPerRole(List<ChampionStatsDto> championStats, List<ChampionDto> championList, FormattedEmbedBuilder message)
        {
            foreach (LeagueRole leagueRole in LeagueRole.Values())
            {
                List<ChampionStatsDto> roleStats = championStats.Where(x => x.Role.Equals(leagueRole.ApiRole)).ToList();
                FormattedTextBuilder winrates = new FormattedTextBuilder();
                int entryCount = 3;
                for (int i = 0; i < entryCount; i++)
                {
                    ChampionStatsDto topX = roleStats.ElementAtOrDefault(i);
                    if (topX != null)
                    {
                        ChampionDto champion = championList.FirstOrDefault(x => x.Id.Equals(topX.ChampionId));
                        if (champion != null)
                        {
                            if (i > 0)
                            {
                                winrates.AppendNewLine();
                            }
                            winrates.Append($"`{this.BuildPercentage(topX.WinRate)}%` - {champion.Name}");
                        }
                    }
                }
                winrates.AppendNewLine().Append("`...`");
                for (int i = roleStats.Count - entryCount; i < roleStats.Count; i++)
                {
                    ChampionStatsDto bottomX = roleStats.ElementAtOrDefault(i);
                    if (bottomX != null)
                    {
                        ChampionDto champion = championList.FirstOrDefault(x => x.Id.Equals(bottomX.ChampionId));
                        if (champion != null)
                        {
                            winrates.AppendNewLine().Append($"`{this.BuildPercentage(bottomX.WinRate)}%` - {champion.Name}");
                        }
                    }
                }
                message.AddField(leagueRole.Name, winrates.ToString(), new AppendOption[] { AppendOption.Underscore });
            }
        }

        private string BuildPercentage(double percentage)
        {
            return Math.Round(percentage * 100, 2, MidpointRounding.AwayFromZero).ToString("#.00", CultureInfo.InvariantCulture);
        }

        // ---------------------------------------------------------

        private async Task<List<ChampionDto>> GetMatchingChampions(string name)
        {
            name = name.ToLower();
            ChampionListDto championList = await this._riotStaticData.GetChampionsAsync();
            List<ChampionDto> matches = new List<ChampionDto>();
            if (championList != null)
            {
                matches = championList.Data.Values
                    .Where(x => x.Name.ToLower().Contains(name) || StripName(x.Name).ToLower().Contains(name)).ToList();
            }
            matches.Sort((a, b) => a.Name.CompareTo(b.Name));
            return matches;
        }

        private async Task<ChampionDto> GetChampionAsync(int championId)
        {
            return await this._riotStaticData.GetChampionAsync(championId);
        }

        private string StripName(string name)
        {
            return Regex.Replace(name, @"[^ a-zA-Z0-9]+", string.Empty);
        }
    }
}
