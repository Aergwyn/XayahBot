﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using XayahBot.API.Model;
using XayahBot.Utility;

namespace XayahBot.Service
{
    public static class QuizService
    {
        private static SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);
        private static Dictionary<ulong, QuizEntry> _questionMap = new Dictionary<ulong, QuizEntry>(); // Saves question per guild

        //

        private static List<string> _fullAnswerList = new List<string>() {
            "Wow. {0} was actually correct.",
            "{0} was first to provide the right answer.",
            "{0} won the cookie!",
        };
        private static List<string> _partialAnswerList = new List<string>(){
            "Wow. {0} was actually somewhat right. {1} would've been the full answer.",
            "{0} barely made it. Actually it was {1} but I'm nice.",
            "*yawn* {0} finally did it! Guessing {1} isn't so hard, no?",
        };

        private static string _questionTimeout = "Oh. So much time passed and no one was able to guess it. {0} would've done it though. *sigh*";
        private static string _questionMaxTries = "Seems like this question was too hard. No one knew the answer was {0}. Oh well.";

        private static string _championBasicNameQ = "Which champion got the title \"{0}\"?";
        private static string _championBasicTitleQ = "Name the title of {0} with at least {1}% accuracy!";
        private static string _championBasicResourceQ = "What is {0}'s resource called?";
        private static string _championBasicTagQ = "List one tag of {0}!";
        private static string _championSkinNameQ = "Name a skin of {0} with at least {1}% accuracy! Also ignore the classic skin. That'd be too easy.";
        private static string _championSkinCountQ = "How many skins does {0} have in total?";
        private static string _championSkinOrderQ = "Name the {0}{1} released skin for {2}? {3}% accuracy is enough. Oh, and exclude the classic skin. It doesn't count.";
        private static string _championSpellNameQ = "Name one spell (or passive) of {0}!";
        private static string _championSpellCdQ = "What is the base cooldown on a rank {0} {1} from {2}?";
        private static string _championSpellRangeQ = "What is the range of a rank {0} {1} from {2}?";
        private static string _championSpellCostQ = "What is the cost of a rank {0} {1} from {2}?";
        private static string _championSpellCoEffQ = "Name a scaling (in %) of a rank {0} {1} from {2}!";
        private static string _championStatsArmorQ = "How much armor does {0} have at level {1}?";
        private static string _championStatsRangeQ = "What is the basic attack range of {0}?";
        private static string _championStatsHpQ = "How much health points does {0} have at level {1}?";
        private static string _championStatsSpeedQ = "What is the base move speed of {0}?";
        private static string _championStatsMpQ = "How much mana points does {0} have at level {1}?";
        private static string _championStatsMrQ = "How much magic resist does {0} have at level {1}?";

        //

#pragma warning disable 4014 // Intentional
        public static async Task AskQuestionAsync(CommandContext context)
        {
            string question = string.Empty;
            await _syncLock.WaitAsync();
            try
            {
                if (_questionMap.TryGetValue(context.Guild.Id, out QuizEntry entry))
                {
                    if (entry.TimeAsked.AddMinutes(int.Parse(Property.QuizTimeout.Value)) < DateTime.UtcNow)
                    {
                        _questionMap.Remove(context.Guild.Id);
                        question = string.Format(_questionTimeout, entry.GetAllAnswers());
                    }
                    else
                    {
                        question = entry.Question;
                    }
                }
                else
                {
                    entry = await AskChampionAsync();
                    if (entry != null)
                    {
                        question = entry.Question;
                        _questionMap.Add(context.Guild.Id, entry);
                    }
                }
                if (!string.IsNullOrWhiteSpace(question))
                {
                    context.Channel.SendMessageAsync(question);
                }
                else
                {
                    Logger.Log(LogSeverity.Error, nameof(QuizService), "Code returned with no question!");
                }
            }
            finally
            {
                _syncLock.Release();
            }
        }
#pragma warning restore 4014

#pragma warning disable 4014 // Intentional
        public static async Task AnswerQuestionAsync(CommandContext context, string answer)
        {
            bool success = false;
            string response = string.Empty;
            answer = answer.Trim().ToLower();
            await _syncLock.WaitAsync();
            try
            {
                if (_questionMap.TryGetValue(context.Guild.Id, out QuizEntry entry))
                {
                    string correctAnswer = string.Empty;
                    if (entry.MatchPercentage >= 100) // Exact Match
                    {
                        correctAnswer = entry.Answer.FirstOrDefault(x => x.ToLower().Equals(answer));
                        if (!string.IsNullOrWhiteSpace(correctAnswer))
                        {
                            success = true;
                            _questionMap.Remove(context.Guild.Id);
                            response = string.Format(GetRandomAnswer(_fullAnswerList), context.User.Mention);
                        }
                    }
                    else // Partial Match
                    {
                        correctAnswer = entry.Answer.FirstOrDefault(x => x.ToLower().Contains(answer));
                        if (!string.IsNullOrWhiteSpace(correctAnswer)) // Check if answer given is at least occuring in a correct answer
                        {
                            int percentage = (int)Math.Round((decimal)answer.Length / correctAnswer.Length * 100, 0, MidpointRounding.AwayFromZero);
                            if (percentage >= 100)
                            {
                                success = true;
                                _questionMap.Remove(context.Guild.Id);
                                response = string.Format(GetRandomAnswer(_fullAnswerList), context.User.Mention);
                            }
                            else if (percentage >= int.Parse(Property.QuizMatch.Value))
                            {
                                success = true;
                                _questionMap.Remove(context.Guild.Id);
                                response = string.Format(GetRandomAnswer(_partialAnswerList), context.User.Mention, correctAnswer);
                            }
                        }
                    }
                    if (!success)
                    {
                        entry.TimesFailed++;
                        if (entry.TimesFailed >= int.Parse(Property.QuizMaxTries.Value))
                        {
                            _questionMap.Remove(context.Guild.Id);
                            response = string.Format(_questionMaxTries, entry.GetAllAnswers());
                        }
                        else if (entry.TimeAsked.AddMinutes(int.Parse(Property.QuizTimeout.Value)) < DateTime.UtcNow)
                        {
                            _questionMap.Remove(context.Guild.Id);
                            response = string.Format(_questionTimeout, entry.GetAllAnswers());
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(response))
                {
                    context.Channel.SendMessageAsync(response);
                }
            }
            finally
            {
                _syncLock.Release();
            }
        }
#pragma warning restore 4014

        //

        private static string GetRandomAnswer(List<string> list)
        {
            return list.ElementAt(RNG.Next(list.Count()) - 1);
        }

        //

        private static async Task<QuizEntry> AskChampionAsync()
        {
            QuizEntry entry = null;
            ChampionListDto championList = await RiotDataService.GetChampionListAsync();
            int id = championList.Data.ElementAt(RNG.Next(championList.Data.Count()) - 1).Value.Id; // Get random champion id from list
            ChampionDto champion = await RiotDataService.GetChampionDetailsAsync(id); // Get/Update data of champion
            if (champion != null)
            {
                switch (RNG.Next(4))
                {
                    case 1:
                        entry = AskChampionBasic(champion);
                        break;
                    case 2:
                        entry = AskChampionSkin(champion);
                        break;
                    case 3:
                        entry = AskChampionSpell(champion);
                        break;
                    case 4:
                        entry = AskChampionStats(champion);
                        break;
                }
            }
            return entry;
        }

        private static QuizEntry AskChampionBasic(ChampionDto champion)
        {
            QuizEntry entry = null;
            switch (RNG.Next(4))
            {
                case 1:
                    entry = new QuizEntry(string.Format(_championBasicNameQ, champion.Title, Property.QuizMatch), int.Parse(Property.QuizMatch.Value), champion.Name);
                    break;
                case 2:
                    entry = new QuizEntry(string.Format(_championBasicTitleQ, champion.Name), champion.Title);
                    break;
                case 3:
                    entry = new QuizEntry(string.Format(_championBasicResourceQ, champion.Name), champion.ParType);
                    break;
                case 4:
                    entry = new QuizEntry(string.Format(_championBasicTagQ, champion.Name), champion.Tags.ToArray());
                    break;
            }
            return entry;
        }

        private static QuizEntry AskChampionSkin(ChampionDto champion)
        {
            QuizEntry entry = null;
            switch (RNG.Next(3))
            {
                case 1:
                    List<string> answer = new List<string>();
                    foreach(SkinDto skin in champion.Skins.Where(x => x.Num > 0))
                    {
                        answer.Add(skin.Name);
                    }
                    entry = new QuizEntry(string.Format(_championSkinNameQ, champion.Name, Property.QuizMatch), int.Parse(Property.QuizMatch.Value), answer.ToArray());
                    break;
                case 2:
                    entry = new QuizEntry(string.Format(_championSkinCountQ, champion.Name), champion.Skins.Count().ToString());
                    break;
                case 3:
                    SkinDto randomSkin = champion.Skins.FirstOrDefault(x => x.Num.Equals(RNG.Next(champion.Skins.Count() - 1)));
                    entry = new QuizEntry(string.Format(_championSkinOrderQ, randomSkin.Num, NumberSuffix(randomSkin.Num), champion.Name, Property.QuizMatch), int.Parse(Property.QuizMatch.Value), randomSkin.Name);
                    break;
            }
            return entry;
        }

        private static string NumberSuffix(int number)
        {
            switch (number)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }

        private static QuizEntry AskChampionSpell(ChampionDto champion)
        {
            QuizEntry entry = null;
            ChampionSpellDto spell = champion.Spells.ElementAt(RNG.Next(champion.Spells.Count()) - 1); // Get random spell from list
            int rank = RNG.Next(spell.MaxRank); // Random spell rank
            switch (RNG.Next(5))
            {
                case 1:
                    List<string> answer = new List<string>();
                    foreach (ChampionSpellDto spellDto in champion.Spells)
                    {
                        answer.Add(spellDto.Name);
                    }
                    answer.Add(champion.Passive.Name);
                    entry = new QuizEntry(string.Format(_championSpellNameQ, champion.Name), answer.ToArray());
                    break;
                case 2:
                    entry = new QuizEntry(string.Format(_championSpellCdQ, rank, spell.Name, champion.Name), spell.Cooldown.ElementAt(rank - 1).ToString());
                    break;
                case 3:
                    entry = new QuizEntry(string.Format(_championSpellRangeQ, rank, spell.Name, champion.Name), AnswerToRangeQuestion(spell, rank - 1));
                    break;
                case 4:
                    entry = new QuizEntry(string.Format(_championSpellCostQ, rank, spell.Name, champion.Name), spell.Cost.ElementAt(rank - 1).ToString());
                    break;
                case 5:
                    entry = new QuizEntry(string.Format(_championSpellCoEffQ, rank, spell.Name, champion.Name), AnswerToCoEffQuestion(spell, rank - 1));
                    break;
            }
            return entry;
        }

        private static string[] AnswerToRangeQuestion(ChampionSpellDto spell, int rank)
        {
            List<string> answer = new List<string>();
            try
            {
                JArray range = (JArray)spell.Range; // *shrug*
                answer.Add(range.ElementAt(rank).ToString());
            }
            catch (InvalidCastException)
            {
                answer.Add("0");
                answer.Add("Self");
            }
            return answer.ToArray();
        }

        private static string[] AnswerToCoEffQuestion(ChampionSpellDto spell, int rank)
        {
            List<string> answer = new List<string>();
            if (spell.Vars != null)
            {
                foreach (SpellVarsDto spellVar in spell.Vars)
                {
                    foreach (decimal coEff in spellVar.CoEff)
                    {
                        int value = (int)Math.Round(coEff * 100, 0, MidpointRounding.AwayFromZero);
                        answer.Add(value.ToString());
                    }
                }
            }
            if (answer.Count() == 0)
            {
                answer.Add("0");
                answer.Add("None");
            }
            return answer.ToArray();
        }

        private static QuizEntry AskChampionStats(ChampionDto champion)
        {
            QuizEntry entry = null;
            StatsDto stats = champion.Stats;
            int level = RNG.Next(18);
            switch (RNG.Next(6))
            {
                case 1:
                    entry = new QuizEntry(string.Format(_championStatsArmorQ, champion.Name, level), ((int)Math.Round(stats.Armor + (stats.ArmorPerLevel * (level - 1)), 0, MidpointRounding.AwayFromZero)).ToString());
                    break;
                case 2:
                    entry = new QuizEntry(string.Format(_championStatsRangeQ, champion.Name), ((int)stats.AttackRange).ToString());
                    break;
                case 3:
                    entry = new QuizEntry(string.Format(_championStatsHpQ, champion.Name, level), ((int)Math.Round(stats.Hp + (stats.HpPerLevel * (level - 1)), 0, MidpointRounding.AwayFromZero)).ToString());
                    break;
                case 4:
                    entry = new QuizEntry(string.Format(_championStatsSpeedQ, champion.Name), ((int)stats.MoveSpeed).ToString());
                    break;
                case 5:
                    entry = new QuizEntry(string.Format(_championStatsMpQ, champion.Name, level), ((int)Math.Round(stats.Mp + (stats.MpPerLevel * (level - 1)), 0, MidpointRounding.AwayFromZero)).ToString());
                    break;
                case 6:
                    entry = new QuizEntry(string.Format(_championStatsMrQ, champion.Name, level), ((int)Math.Round(stats.Spellblock + (stats.SpellblockPerLevel * (level - 1)), 0, MidpointRounding.AwayFromZero)).ToString());
                    break;
            }
            return entry;
        }
    }
}