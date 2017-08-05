using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XayahBot.API.Riot;
using XayahBot.API.Riot.Model;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Data
{
    public static class ChampionDataBuilder
    {
        private static readonly RiotStaticData _riotStaticData = new RiotStaticData(Region.EUW);

        public static async Task<DiscordFormatEmbed> BuildAsync(string name)
        {
            name = name.Trim();
            DiscordFormatEmbed message = new DiscordFormatEmbed();
            List<ChampionDto> matches = await GetMatchingChampions(name);
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
                ChampionDto champion = await _riotStaticData.GetChampionAsync(matches.First().Id);
                AppendMiscData(champion, message);
                AppendStatisticData(champion, message);
                AppendSpellData(champion, message);
                AppendSkinData(champion, message);
            }
            return message;
        }

        private static async Task<List<ChampionDto>> GetMatchingChampions(string name)
        {
            name = name.ToLower();
            ChampionListDto championList = await _riotStaticData.GetChampionsAsync();
            List<ChampionDto> matches = new List<ChampionDto>();
            if (championList != null)
            {
                matches = championList.Data.Values
                    .Where(x => x.Name.ToLower().Contains(name) || StripName(x.Name).ToLower().Contains(name)).ToList();
            }
            matches.Sort((a, b) => a.Name.CompareTo(b.Name));
            return matches;
        }

        private static string StripName(string name)
        {
            return Regex.Replace(name, @"[^ a-zA-Z0-9]+", string.Empty);
        }

        private static void AppendMiscData(ChampionDto champion, DiscordFormatEmbed message)
        {
            champion.Tags.Sort();
            message
                .SetThumbnail($"http://ddragon.leagueoflegends.com/cdn/{Property.RiotUrlVersion.Value}/img/champion/{champion.Name}.png")
                .AppendTitle($"{champion.Name} {champion.Title}", AppendOption.Bold, AppendOption.Underscore)
                .AppendDescription("Classified as:", AppendOption.Italic)
                .AppendDescription($" {string.Join(", ", champion.Tags)}")
                .AppendDescription(Environment.NewLine)
                .AppendDescription("Resource:", AppendOption.Italic)
                .AppendDescription($" {champion.Resource}")
                .AppendDescription(Environment.NewLine)
                .AppendDescription("Passive:", AppendOption.Italic)
                .AppendDescription($" {champion.Passive.Name}");
        }

        private static void AppendStatisticData(ChampionDto champion, DiscordFormatEmbed message)
        {
            string text = string.Empty;
            StatsDto stats = champion.Stats;
            DecimalAlign statList = new DecimalAlign(stats.GetStats());
            DecimalAlign statGrowthList = new DecimalAlign(stats.GetStatGrowthList());
            text +=
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
            DiscordFormatMessage statData = new DiscordFormatMessage()
                .Append(text, AppendOption.Codeblock);
            message.AddField("Stats", statData.ToString(), false, FieldFormatType.NAME, AppendOption.Underscore);
        }

        private static void AppendSpellData(ChampionDto champion, DiscordFormatEmbed message)
        {
            for(int i = 0; i < champion.Spells.Count; i++)
            {
                ChampionSpellDto spell = champion.Spells.ElementAt(i);
                DiscordFormatMessage spellDetail = new DiscordFormatMessage()
                    .Append("Cost:", AppendOption.Italic)
                    .Append($" {spell.GetCostString()}")
                    .Append(Environment.NewLine)
                    .Append("Range:", AppendOption.Italic)
                    .Append($" {spell.GetRangeString()}")
                    .Append(Environment.NewLine)
                    .Append("Cooldown:", AppendOption.Italic)
                    .Append($" {spell.GetCooldownString()}")
                    .Append(Environment.NewLine)
                    .Append("Scaling:", AppendOption.Italic)
                    .Append($" {spell.GetVarsString()}");
                message.AddField($"{GetSpellKey(i)} - {spell.Name}", spellDetail.ToString(), true, FieldFormatType.NAME, AppendOption.Underscore);
            }
        }

        private static string GetSpellKey(int position)
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

        private static void AppendSkinData(ChampionDto champion, DiscordFormatEmbed message)
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
            message.AddField("Skins", skinData, false, FieldFormatType.NAME, AppendOption.Underscore);
        }
    }
}
