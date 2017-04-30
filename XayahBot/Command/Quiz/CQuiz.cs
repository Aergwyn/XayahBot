#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Command.System;
using XayahBot.Database.Model;
using XayahBot.Database.Service;
using XayahBot.Utility;

namespace XayahBot.Command.Quiz
{
    [Group("quiz")]
    public class CQuiz : ModuleBase
    {
        private readonly List<string> _recentlyPosted = new List<string>
        {
            "... I just posted it. Not even a few minutes ago. Go look for it!",
            "I refuse to post it again!",
            "Again? But I just did!"
        };
        private readonly List<string> _emptyLeaderboard = new List<string>
        {
            "Unlucky. No one had any success so far.",
            "I could show something if anyone would be able to answer the questions.",
            "The leaderboard is empty so far."
        };

        //

        private static DateTime _lastLeaderboardPost;

        private readonly RNG _random = new RNG();
        private readonly LeaderboardDAO _leaderboardDao = new LeaderboardDAO();

        [Command("ask")]
        [RequireContext(ContextType.Guild)]
        [CheckIgnoredUser]
        [CheckIgnoredChannel]
        [Summary("Asks a random question about a champion.")]
        public Task Ask()
        {
            QuizService.AskQuestionAsync(this.Context);
            return Task.CompletedTask;
        }

        [Command("stats")]
        [RequireContext(ContextType.Guild)]
        [CheckIgnoredUser]
        [CheckIgnoredChannel]
        [Summary("Displays the quiz leaderboard for this month.")]
        public async Task Stats()
        {
            string message = string.Empty;
            List<TLeaderboardEntry> leaderboard = (await this._leaderboardDao.GetLeaderboardAsync(this.Context.Guild.Id)).OrderByDescending(x => x.Answers).ToList();
            if (leaderboard.Count > 0)
            {
                if (_lastLeaderboardPost.AddMinutes(int.Parse(Property.QuizLeaderboardCd.Value)) < DateTime.UtcNow)
                {
                    int width = leaderboard.First().Answers.ToString().Length;
                    message = "Most correctly answered questions this month...```";
                    for (int i = 0; i < leaderboard.Count && i < int.Parse(Property.QuizLeaderboardMax.Value); i++)
                    {
                        if (i > 0)
                        {
                            message += Environment.NewLine;
                        }
                        TLeaderboardEntry entry = leaderboard.ElementAt(i);
                        message += $"{entry.Answers.ToString().PadLeft(width)} - {entry.UserName}";
                    }
                    message += "```";
                    _lastLeaderboardPost = DateTime.UtcNow;
                }
                else
                {
                    message = this._random.FromList(this._recentlyPosted);
                }
            }
            else
            {
                message = this._random.FromList(this._emptyLeaderboard);
            }
            ReplyAsync(message);
        }
    }
}
