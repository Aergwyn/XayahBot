#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Utility;

namespace XayahBot.Database.Service
{
    public class LeaderboardService
    {
        public async Task<List<TLeaderboardEntry>> GetLeaderboard(ulong guild)
        {
            await this.CheckForResetAsync();
            using (GeneralContext database = new GeneralContext())
            {
                return database.Leaderboard.Where(x => x.Guild.Equals(guild)).ToList();
            }
        }

        public async Task IncrementAnswerAsync(ulong guild, ulong userId, string userName)
        {
            await this.CheckForResetAsync();
            using (GeneralContext database = new GeneralContext())
            {
                TLeaderboardEntry match = database.Leaderboard.FirstOrDefault(x => x.Guild.Equals(guild) && x.UserName.Equals(userName));
                if (match == null)
                {
                    database.Leaderboard.Add(new TLeaderboardEntry { Guild = guild, UserId = userId, UserName = userName, Answers = 1 });
                }
                else
                {
                    match.Answers++;
                }
                database.SaveChangesAsync();
            }
        }

        public Task ResetAsync()
        {
            using (GeneralContext database = new GeneralContext())
            {
                database.Leaderboard.RemoveRange(database.Leaderboard);
                database.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }

        //

        private async Task CheckForResetAsync()
        {
            if (!string.IsNullOrWhiteSpace(Property.QuizLastReset.Value))
            {
                DateTime now = DateTime.UtcNow;
                string[] lastReset = Property.QuizLastReset.Value.Split('/');
                if (new DateTime(int.Parse(lastReset.ElementAt(1)), int.Parse(lastReset.ElementAt(0)), 1, 0, 0, 0).AddMonths(1) < now)
                {
                    await ResetAsync();
                    Property.QuizLastReset.Value = $"{now.Month}/{now.Year}";
                }
            }
        }
    }
}
