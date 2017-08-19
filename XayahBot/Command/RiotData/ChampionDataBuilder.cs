using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XayahBot.API.Riot;
using XayahBot.API.Riot.Model;
using XayahBot.Error;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.RiotData
{
    public class ChampionDataBuilder
    {
        public static async Task<FormattedEmbedBuilder> BuildAsync(string name)
        {
            name = name.Trim();
            ChampionDataBuilder championBuilder = new ChampionDataBuilder();
            FormattedEmbedBuilder message = new FormattedEmbedBuilder();
            try
            {
                List<ChampionDto> matches = await championBuilder.GetMatchingChampionsAsync(name);
                if (matches.Count == 0)
                {
                    message
                        .AppendTitle($"{XayahReaction.Error} This didn't work")
                        .AppendDescription($"Oops! Your bad. I couldn't find a champion (fully or partially) named `{name}`.");
                }
                else if (matches.Count > 1)
                {
                    message
                        .AppendTitle($"{XayahReaction.Warning} This didn't went as expected")
                        .AppendDescription($"I found more than one champion (fully or partially) named `{name}`.");
                    foreach (ChampionDto champion in matches)
                    {
                        message.AppendDescription(Environment.NewLine + champion.Name);
                    }
                }
                else
                {
                    ChampionDto champion = await championBuilder.GetChampionAsync(matches.First().Id);
                    championBuilder.AppendMiscData(champion, message);
                    championBuilder.AppendStatisticData(champion, message);
                    championBuilder.AppendSpellData(champion, message);
                    championBuilder.AppendSkinData(champion, message);
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

        private readonly RiotStaticData _riotStaticData = new RiotStaticData(Region.EUW);

        private ChampionDataBuilder()
        {
        }

        private async Task<List<ChampionDto>> GetMatchingChampionsAsync(string name)
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

        private void AppendMiscData(ChampionDto champion, FormattedEmbedBuilder message)
        {
            champion.Tags.Sort();
            message
                .SetThumbnail($"http://ddragon.leagueoflegends.com/cdn/{Property.RiotUrlVersion.Value}/img/champion/{champion.Name}.png")
                .AppendTitle($"{champion.Name} {champion.Title}", AppendOption.Bold, AppendOption.Underscore)
                .AppendDescription("Classified as:", AppendOption.Italic)
                .AppendDescription($" {string.Join(", ", champion.Tags)}")
                .AppendDescriptionNewLine()
                .AppendDescription("Resource:", AppendOption.Italic)
                .AppendDescription($" {champion.Resource}")
                .AppendDescriptionNewLine()
                .AppendDescription("Passive:", AppendOption.Italic)
                .AppendDescription($" {champion.Passive.Name}");
        }

        private void AppendStatisticData(ChampionDto champion, FormattedEmbedBuilder message)
        {
            StatsDto stats = champion.Stats;
            int decimals = 3;
            NumberAlign statList = new NumberAlign(decimals, stats.GetStats());
            NumberAlign statGrowthList = new NumberAlign(decimals, stats.GetStatGrowthList());
            string statData =
                $"Health         - {statList.Align(stats.Hp)} | + {statGrowthList.TrimmedAlign(stats.HpPerLevel)}" +
                Environment.NewLine +
                $"Health Regen.  - {statList.Align(stats.HpRegen)} | + {statGrowthList.TrimmedAlign(stats.HpRegenPerLevel)}" +
                Environment.NewLine +
                $"Mana           - {statList.Align(stats.Mp)} | + {statGrowthList.TrimmedAlign(stats.MpPerLevel)}" +
                Environment.NewLine +
                $"Mana Regen     - {statList.Align(stats.MpRegen)} | + {statGrowthList.TrimmedAlign(stats.MpRegenPerLevel)}" +
                Environment.NewLine +
                $"Attack Damage  - {statList.Align(stats.AttackDamage)} | + {statGrowthList.TrimmedAlign(stats.AttackDamagePerLevel)}" +
                Environment.NewLine +
                $"Attack Speed   - {statList.Align(stats.GetBaseAttackSpeed())} | + {statGrowthList.TrimmedAlign(stats.AttackSpeedPerLevel)}%" +
                Environment.NewLine +
                $"Armor          - {statList.Align(stats.Armor)} | + {statGrowthList.TrimmedAlign(stats.ArmorPerLevel)}" +
                Environment.NewLine +
                $"Magic Resist   - {statList.Align(stats.Spellblock)} | + {statGrowthList.TrimmedAlign(stats.SpellblockPerLevel)}" +
                Environment.NewLine +
                $"Attack Range   - {statList.Align(stats.AttackRange)}" +
                Environment.NewLine +
                $"Movement Speed - {statList.Align(stats.MoveSpeed)}";
            message.AddField("Stats", statData,
                new AppendOption[] { AppendOption.Underscore },
                new AppendOption[] { AppendOption.Codeblock }, inline: false);
        }

        private void AppendSpellData(ChampionDto champion, FormattedEmbedBuilder message)
        {
            for(int i = 0; i < champion.Spells.Count; i++)
            {
                ChampionSpellDto spell = champion.Spells.ElementAt(i);
                FormattedTextBuilder spellDetail = new FormattedTextBuilder()
                    .Append("Cost:", AppendOption.Italic)
                    .Append($" {spell.GetCostString()}")
                    .AppendNewLine()
                    .Append("Range:", AppendOption.Italic)
                    .Append($" {spell.GetRangeString()}")
                    .AppendNewLine()
                    .Append("Cooldown:", AppendOption.Italic)
                    .Append($" {spell.GetCooldownString()}");
                message.AddField($"{GetSpellKey(i)} - {spell.Name}", spellDetail.ToString(),
                    new AppendOption[] { AppendOption.Underscore }, inline: false);
            }
        }

        private string GetSpellKey(int position)
        {
            int result = position % 4;
            switch (result) {
                case 0:
                    return "Q";
                case 1:
                    return "W";
                case 2:
                    return "E";
                case 3:
                    return "R";
                default:
                    return "-";
            }
        }

        private void AppendSkinData(ChampionDto champion, FormattedEmbedBuilder message)
        {
            List<SkinDto> skins = champion.Skins.Where(x => x.Num > 0).ToList();
            skins.Sort((a, b) => a.Num.CompareTo(b.Num));
            string skinData = string.Empty;
            foreach (SkinDto skin in skins)
            {
                string skinNum = skin.Num.ToString();
                while (skinNum.Length < 2)
                {
                    skinNum = "0" + skinNum;
                }
                skinData += $"{Environment.NewLine}`{skinNum}` - {skin.Name}";
            }
            message.AddField("Skins", skinData,
                new AppendOption[] { AppendOption.Underscore }, inline: false);
        }
    }
}
