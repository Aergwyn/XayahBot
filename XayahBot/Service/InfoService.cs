using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using XayahBot.API.Model;
using XayahBot.Utility;

namespace XayahBot.Service
{
    public static class InfoService
    {
        private static readonly string _multipleChampsFound = "Found more than one champion (fully or partially) named `{0}`.{1}";
        private static readonly string _noChampionFound = "Oops! Your bad. I could not find a champion (fully or partially) named `{0}`.";
        private static readonly string _noInputGiven = "You should give me something I can search with...";

        //

#pragma warning disable 4014 // Intentional
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

                        string message = $"{Environment.NewLine}__{champion.Name} {champion.Title}__```" +
                            $"Tags     - {string.Join(", ", champion.Tags)}{Environment.NewLine}" +
                            $"Resource - {champion.ParType}{Environment.NewLine}" +
                            $"Passive  - {champion.Passive.Name}```" +
                            $"{Environment.NewLine}__Abilities__```";
                        for (int i = 0; i < champion.Spells.Count; i++)
                        {
                            if (i > 0)
                            {
                                message += $"{Environment.NewLine}------------{Environment.NewLine}";
                            }
                            ChampionSpellDto spell = champion.Spells.ElementAt(i);
                            message += $"Name     - {spell.Name}{Environment.NewLine}" +
                                $"Cost     - {spell.CostBurn}{Environment.NewLine}" +
                                $"Range    - {spell.RangeBurn}{Environment.NewLine}" +
                                $"Cooldown - {spell.CooldownBurn}{Environment.NewLine}" +
                                $"Scalings - {spell.GetVarsString()}";
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
#pragma warning restore 4014

        //

        private static string FilterName(string name)
        {
            return Regex.Replace(name, @"[^ a-zA-Z0-9]+", string.Empty);
        }
    }
}
