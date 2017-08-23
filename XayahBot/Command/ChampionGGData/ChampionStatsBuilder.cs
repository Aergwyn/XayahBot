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
        private readonly ChampionGGChampions _championGGChampions = new ChampionGGChampions();
        private readonly RiotStaticData _riotStaticData = new RiotStaticData(Region.EUW);
        private List<ChampionDto> _championList = new List<ChampionDto>();
        private List<ChampionStatsDto> _championStats = new List<ChampionStatsDto>();

        public async Task<FormattedEmbedBuilder> BuildForRoleAsync(LeagueRole role)
        {
            FormattedEmbedBuilder message = new FormattedEmbedBuilder();
            try
            {
                await this.LoadFromApi();

                if (LeagueRole.All.Equals(role))
                {
                    foreach (LeagueRole leagueRole in LeagueRole.Values())
                    {
                        this.AppendStatsOfRole(leagueRole, 3, message);
                    }
                }
                else
                {
                    this.AppendStatsOfRole(role, 6, message);
                }
                message.AppendTitle($"{XayahReaction.Clipboard} Winrates");
                ChampionStatsDto first = this._championStats.ElementAtOrDefault(0);
                if (first != null)
                {
                    message.AppendTitle($" for Patch {first.Patch}");
                }
                message.AppendDescription("Short explanation of the table: `Position - Win % - Play %` - Name", AppendOption.Italic);
            }
            catch (NoApiResultException)
            {
                message = new FormattedEmbedBuilder()
                    .AppendTitle($"{XayahReaction.Error} This didn't work")
                    .AppendDescription("Apparently some random API refuses cooperation. Have some patience while I convince them again...");
            }
            return message;
        }

        private async Task LoadFromApi()
        {
            this._championList.AddRange((await this._riotStaticData.GetChampionsAsync()).Data.Values);
            this._championStats.AddRange(await this._championGGChampions.GetChampionsAsync());
            this._championStats.Sort((a, b) => b.WinRate.CompareTo(a.WinRate));
        }

        private void AppendStatsOfRole(LeagueRole role, int entryCount, FormattedEmbedBuilder message)
        {
            List<ChampionStatsDto> roleStats = this._championStats.Where(x => x.Role.Equals(role.ApiRole)).ToList();
            if (entryCount * 2 > roleStats.Count)
            {
                entryCount = (int)Math.Truncate(roleStats.Count / (decimal)2);
            }
            FormattedTextBuilder winrates = new FormattedTextBuilder();
            for (int i = 0; i < entryCount && i < roleStats.Count; i++)
            {
                ChampionStatsDto topX = roleStats.ElementAt(i);
                string championLine = this.BuildChampionStats(topX, i + 1);
                if (i > 0)
                {
                    winrates.AppendNewLine();
                }
                winrates.Append(championLine);
            }
            if (roleStats.Count > entryCount * 2)
            {
                winrates.AppendNewLine();
            }
            for (int i = roleStats.Count - entryCount; i >= 0 && i < roleStats.Count; i++)
            {
                ChampionStatsDto bottomX = roleStats.ElementAt(i);
                string championLine = this.BuildChampionStats(bottomX, i + 1);
                winrates.AppendNewLine().Append(championLine);
            }
            string resultText = winrates.ToString();
            if (string.IsNullOrWhiteSpace(resultText))
            {
                resultText = ". . .";
            }
            message.AddField(role.Name, resultText, new AppendOption[] { AppendOption.Underscore });
        }

        private string BuildChampionStats(ChampionStatsDto championStats, int position)
        {
            ChampionDto champion = this._championList.FirstOrDefault(x => x.Id.Equals(championStats.ChampionId));
            if (champion != null)
            {
                return $"`{position.ToString("00", CultureInfo.InvariantCulture)} - {this.FormatPercentage(championStats.WinRate)} - " +
                    $"{this.FormatPercentage(championStats.PlayRate)}` - {champion.Name}";
            }
            return string.Empty;
        }

        private string FormatPercentage(double percentage)
        {
            double result = Math.Round(percentage * 100, 2, MidpointRounding.AwayFromZero);
            return result.ToString("#.00", CultureInfo.InvariantCulture).PadLeft(5, ' ');
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
