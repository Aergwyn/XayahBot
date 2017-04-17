using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Service;
using XayahBot.Database.Model;
using System.Collections.Generic;
using XayahBot.Database.Service;
using System.Linq;
using System;
using XayahBot.Utility;

namespace XayahBot.Command
{
    [Group("quiz")]
    public class CQuiz : ModuleBase
    {
        private static DateTime _lastLeaderboardPost;

        //

        private readonly List<string> _emptyLeaderboard = new List<string>{
            "Unlucky. No one had any success so far.",
            "I could show something if anyone would be able to answer the questions.",
            "The leaderboard is empty so far."
        };

        private readonly List<string> _recentlyPosted = new List<string>{
            "... I just posted it. Not even a few minutes ago. Go look for it!",
            "I refuse to post it again!",
            "Again? But I just did!"
        };

        //

#pragma warning disable 4014 // Intentional
        [Command]
        [RequireContext(ContextType.Guild)]
        [Summary("Asks a random question about a champion.")]
        public Task Quiz()
        {
            QuizService.AskQuestionAsync(this.Context);
            return Task.CompletedTask;
        }
#pragma warning restore 4014

        [Command("stats")]
        [RequireContext(ContextType.Guild)]
        [Summary("Lists users in descending order regarding their correct answers this month.")]
        public Task Stats()
        {
            string message = string.Empty;
            List<TQuizStat> leaderboard = QuizStatService.GetStatList(this.Context.Guild.Id).OrderByDescending(x => x.Answers).ToList();
            if (leaderboard.Count > 0)
            {
                if (_lastLeaderboardPost.AddMinutes(int.Parse(Property.QuizLeaderboardCd.Value)) < DateTime.UtcNow)
                {
                    int width = leaderboard.First().Answers.ToString().Length;
                    message = "Most correctly answered questions this month...```";
                    for (int i = 0; i < leaderboard.Count; i++)
                    {
                        if (i > 0)
                        {
                            message += Environment.NewLine;
                        }
                        TQuizStat stat = leaderboard.ElementAt(i);
                        message += $"{stat.Answers.ToString().PadLeft(width)} - {stat.User}";
                    }
                    message += "```";
                    _lastLeaderboardPost = DateTime.UtcNow;
                }
                else
                {
                    message = this._recentlyPosted.ElementAt(RNG.Next(this._recentlyPosted.Count) - 1);
                }
            }
            else
            {
                 message = this._emptyLeaderboard.ElementAt(RNG.Next(this._emptyLeaderboard.Count) - 1);
            }
            ReplyAsync(message);
            return Task.CompletedTask;
        }
    }
}
