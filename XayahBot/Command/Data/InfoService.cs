#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XayahBot.API.Riot;
using XayahBot.API.Riot.Model;
using XayahBot.Utility;

namespace XayahBot.Command.Data
{
    public static class InfoService
    {
        public static async Task<DiscordFormatMessage> GetChampionDataText(string name)
        {
            DiscordFormatMessage message = new DiscordFormatMessage();
            name = name.Trim();
            RiotStaticDataApi staticDataApi = new RiotStaticDataApi();
            ChampionListDto championList = await staticDataApi.GetChampionsAsync();
            List<ChampionDto> matches = championList.Data.Values.Where(x => x.Name.ToLower().Contains(name.ToLower()) || FilterName(x.Name).ToLower().Contains(name.ToLower())).ToList();
            matches.Sort((a, b) => a.Name.CompareTo(b.Name));
            if (matches.Count > 1)
            {
                message.Append($"Found more than one champion (fully or partially) named `{name}`.");
                message = BuildMatchListString(matches, message);
            }
            else if (matches.Count > 0)
            {
                ChampionDto champion = await staticDataApi.GetChampionAsync(matches.First().Id);
                message = BuildChampionDataString(champion, message);
                Logger.Debug($"Posting champion data. Message length: {message.ToString().Length}");
            }
            else
            {
                message.Append($"Oops! Your bad. I could not find a champion (fully or partially) named `{name}`.");
            }
            return message;
        }

        private static string FilterName(string name)
        {
            return Regex.Replace(name, @"[^ a-zA-Z0-9]+", string.Empty);
        }

        private static DiscordFormatMessage BuildMatchListString(List<ChampionDto> matches, DiscordFormatMessage message)
        {
            string nameList = string.Empty;
            foreach (ChampionDto champion in matches)
            {
                nameList += Environment.NewLine + champion.Name;
            }
            message.AppendCodeBlock(nameList);
            return message;
        }

        private static DiscordFormatMessage BuildChampionDataString(ChampionDto champion, DiscordFormatMessage message)
        {
            message.Append($"{champion.Name} {champion.Title}", AppendOption.BOLD, AppendOption.UNDERSCORE);
            message.AppendCodeBlock(BuildGeneralString(champion));
            message.Append("Statistics", AppendOption.UNDERSCORE);
            message.AppendCodeBlock(BuildStatisticsString(champion.Stats));
            message.Append($"Abilities", AppendOption.UNDERSCORE);
            message.AppendCodeBlock(BuildAbilitiesString(champion.Spells));
            message.Append("Skins", AppendOption.UNDERSCORE);
            message.AppendCodeBlock(BuildSkinsWithoutDefaultString(champion.Skins));
            return message;
        }

        private static string BuildGeneralString(ChampionDto champion)
        {
            champion.Tags.Sort();
            return $"Tags     - {string.Join(", ", champion.Tags)}{Environment.NewLine}" +
                $"Resource - {champion.ParType}{Environment.NewLine}" +
                $"Passive  - {champion.Passive.Name}";
        }

        private static string BuildStatisticsString(StatsDto stats)
        {
            List<decimal> leftStats = new List<decimal>()
            {
                stats.Hp,
                stats.HpRegen,
                stats.Mp,
                stats.MpRegen,
                stats.AttackRange,
            };
            List<decimal> leftScaling = new List<decimal>()
            {
                stats.HpPerLevel,
                stats.HpRegenPerLevel,
                stats.MpPerLevel,
                stats.MpRegenPerLevel,
                // no AttackRange here
            };
            List<decimal> rightStats = new List<decimal>()
            {
                stats.AttackDamage,
                stats.GetBaseAttackSpeed(),
                stats.Armor,
                stats.Spellblock,
                stats.MoveSpeed,
            };
            List<decimal> rightScaling = new List<decimal>()
            {
                stats.AttackDamagePerLevel,
                stats.AttackSpeedPerLevel,
                stats.ArmorPerLevel,
                stats.SpellblockPerLevel,
                // no MoveSpeed here
            };
            DecAlign leftStatAlign = new DecAlign(leftStats);
            DecAlign leftScalingAlign = new DecAlign(leftScaling);
            DecAlign rightStatAlign = new DecAlign(rightStats);
            DecAlign rightScalingAlign = new DecAlign(rightScaling);

            string text = $"Health    - {leftStatAlign.Align(stats.Hp)} (+ {leftScalingAlign.Align(stats.HpPerLevel)}) | " +
                $"Attack Damage - {rightStatAlign.Align(stats.AttackDamage)} (+ {rightScalingAlign.Align(stats.AttackDamagePerLevel)}){Environment.NewLine}" + 
                $"HP Regen. - {leftStatAlign.Align(stats.HpRegen)} (+ {leftScalingAlign.Align(stats.HpRegenPerLevel)}) | " +
                $"Attack Speed  - {rightStatAlign.Align(stats.GetBaseAttackSpeed())} (+ {rightScalingAlign.Align(stats.AttackSpeedPerLevel)}){Environment.NewLine}" +
                $"Mana      - {leftStatAlign.Align(stats.Mp)} (+ {leftScalingAlign.Align(stats.MpPerLevel)}) | " +
                $"Armor         - {rightStatAlign.Align(stats.Armor)} (+ {rightScalingAlign.Align(stats.ArmorPerLevel)}){Environment.NewLine}" +
                $"MP Regen. - {leftStatAlign.Align(stats.MpRegen)} (+ {leftScalingAlign.Align(stats.MpRegenPerLevel)}) | " +
                $"Magic Resist  - {rightStatAlign.Align(stats.Spellblock)} (+ {rightScalingAlign.Align(stats.SpellblockPerLevel)}){Environment.NewLine}" +
                $"Range     - {leftStatAlign.Align(stats.AttackRange)} {"".PadLeft(leftScalingAlign.GetFieldLength() + 4)} | " +
                $"Move. Speed   - {rightStatAlign.Align(stats.MoveSpeed)}";
            return text;
        }

        private static string BuildAbilitiesString(List<ChampionSpellDto> abilities)
        {
            string text = string.Empty;
            for (int i = 0; i < abilities.Count; i++)
            {
                if (i > 0)
                {
                    text += $"{Environment.NewLine}------------{Environment.NewLine}";
                }
                ChampionSpellDto spell = abilities.ElementAt(i);
                text += $"Name     - {spell.Name}{Environment.NewLine}" +
                    $"Cost     - {spell.GetCostString()}{Environment.NewLine}" +
                    $"Range    - {spell.GetRangeString()}{Environment.NewLine}" +
                    $"Cooldown - {spell.GetCooldownString()}{Environment.NewLine}" +
                    $"Scaling  - {spell.GetVarsString()}";
            }
            return text;
        }

        private static string BuildSkinsWithoutDefaultString(List<SkinDto> skins)
        {
            List<SkinDto> skinWithoutDefault = skins.Where(x => x.Num > 0).ToList();
            skins.Sort((a, b) => a.Num.CompareTo(b.Num));
            string text = string.Empty;
            for (int i = 0; i < skins.Count; i++)
            {
                if (i > 0)
                {
                    text += Environment.NewLine;
                }
                SkinDto skin = skins.ElementAt(i);
                text += $"{skin.Num.ToString().PadLeft(2)} - {skin.Name}";
            }
            return text;
        }
    }
}
