#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using XayahBot.API.Model;
using XayahBot.Utility;
using System.Globalization;

namespace XayahBot.Service
{
    public static class InfoService
    {
        private static readonly string _multipleChampsFound = "Found more than one champion (fully or partially) named `{0}`.{1}";
        private static readonly string _noChampionFound = "Oops! Your bad. I could not find a champion (fully or partially) named `{0}`.";
        private static readonly string _noInputGiven = "You should give me something I can search with...";

        //

        public static async Task GetChampionData(IMessageChannel channel, string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                ChampionListDto championList = await RiotDataService.GetChampionListAsync();

                List<ChampionDto> matches = championList.Data.Values.Where(x => x.Name.ToLower().Contains(name.ToLower()) || FilterName(x.Name).ToLower().Contains(name.ToLower())).ToList();
                matches.Sort((a, b) => a.Name.CompareTo(b.Name));
                if (matches.Count > 0)
                {
                    if (matches.Count > 1)
                    {
                        string nameList = "```";
                        foreach (ChampionDto champion in matches)
                        {
                            nameList += Environment.NewLine + champion.Name;
                        }
                        nameList += "```";
                        channel.SendMessageAsync(string.Format(_multipleChampsFound, name, nameList));
                    }
                    else
                    {
                        ChampionDto champion = await RiotDataService.GetChampionDetailsAsync(matches.ElementAt(0).Id);
                        champion.Tags.Sort();
                        List<SkinDto> skins = champion.Skins.Where(x => x.Num > 0).ToList();
                        skins.Sort((a, b) => a.Num.CompareTo(b.Num));

                        string message = $"__**{champion.Name} {champion.Title}**__```" +
                            $"Tags     - {string.Join(", ", champion.Tags)}{Environment.NewLine}" +
                            $"Resource - {champion.ParType}{Environment.NewLine}" +
                            $"Passive  - {champion.Passive.Name}" +
                            $"```{Environment.NewLine}__Statistics__```" +
                            GetStatisticBlock(champion.Stats) +
                            $"```{Environment.NewLine}__Abilities__```";
                        for (int i = 0; i < champion.Spells.Count; i++)
                        {
                            if (i > 0)
                            {
                                message += $"{Environment.NewLine}------------{Environment.NewLine}";
                            }
                            ChampionSpellDto spell = champion.Spells.ElementAt(i);
                            message += $"Name     - {spell.Name}{Environment.NewLine}" +
                                $"Cost     - {spell.GetCostString()}{Environment.NewLine}" +
                                $"Range    - {spell.GetRangeString()}{Environment.NewLine}" +
                                $"Cooldown - {spell.GetCooldownString()}{Environment.NewLine}" +
                                $"Scaling  - {spell.GetVarsString()}";
                        }
                        message += $"```{Environment.NewLine}__Skins__```";
                        for (int i = 0; i < skins.Count; i++)
                        {
                            if (i > 0)
                            {
                                message += Environment.NewLine;
                            }
                            SkinDto skin = skins.ElementAt(i);
                            message += $"{skin.Num.ToString().PadLeft(2)} - {skin.Name}";
                        }
                        message += "```";

                        channel.SendMessageAsync(message);
                        Logger.Log(LogSeverity.Debug, nameof(InfoService), $"Posting champion data. Message length: {message.Length}");
                    }
                }
                else
                {
                    channel.SendMessageAsync(string.Format(_noChampionFound, name));
                }
            }
            else
            {
                channel.SendMessageAsync(_noInputGiven);
            }
        }

        //

        private static string FilterName(string name)
        {
            return Regex.Replace(name, @"[^ a-zA-Z0-9]+", string.Empty);
        }

        private static string GetStatisticBlock(StatsDto stats)
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
    }
}
