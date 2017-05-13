#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.Precondition;
using XayahBot.Database.DAO;
using XayahBot.Database.Model;
using XayahBot.Utility;

namespace XayahBot.Command.Quiz
{
    [Group("quiz")]
    [Category(CategoryType.QUIZ)]
    public class CQuiz : ModuleBase
    {
        private readonly List<string> _emptyLeaderboard = new List<string>
        {
            "Unlucky. No one had any success so far.",
            "I could show something if anyone would be able to answer the questions.",
            "The leaderboard is empty so far."
        };

        //

        private readonly QuizLeaderboardDAO _quizLeaderboardDao = new QuizLeaderboardDAO();

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
            List<TLeaderboardEntry> leaderboard = (await this._quizLeaderboardDao.GetAllAsync(this.Context.Guild.Id)).OrderByDescending(x => x.Answers).ToList();
            if (leaderboard.Count > 0)
            {
                int width = leaderboard.First().Answers.ToString().Length;
                message = "Most correctly answered questions this month:```";
                for (int i = 0; i < leaderboard.Count && i < int.Parse(Property.QuizLeaderboardMax.Value); i++)
                {
                    if (i > 0)
                    {
                        message += Environment.NewLine;
                    }
                    TLeaderboardEntry entry = leaderboard.ElementAt(i);
                    IUser user = await this.Context.Channel.GetUserAsync(entry.UserId);
                    message += $"{entry.Answers.ToString().PadLeft(width)} - {user}";
                }
                message += "```";
            }
            else
            {
                message = RNG.FromList(this._emptyLeaderboard);
            }
            ReplyAsync(message);
        }
    }
}
