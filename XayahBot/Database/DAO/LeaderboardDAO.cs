#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Utility;

namespace XayahBot.Database.DAO
{
    public class LeaderboardDAO
    {
        public async Task<List<TLeaderboardEntry>> GetLeaderboardAsync(ulong guildId)
        {
            await this.CheckForResetAsync();
            using (GeneralContext database = new GeneralContext())
            {
                return database.QuizLeaderboard.Where(x => x.GuildId.Equals(guildId)).ToList();
            }
        }

        public async Task IncrementAnswerAsync(ulong guildId, ulong userId, string userName)
        {
            await this.CheckForResetAsync();
            using (GeneralContext database = new GeneralContext())
            {
                TLeaderboardEntry match = database.QuizLeaderboard.FirstOrDefault(x => x.GuildId.Equals(guildId) && x.UserName.Equals(userName));
                if (match == null)
                {
                    database.QuizLeaderboard.Add(new TLeaderboardEntry { GuildId = guildId, UserId = userId, UserName = userName, Answers = 1 });
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
                database.QuizLeaderboard.RemoveRange(database.QuizLeaderboard);
                database.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }

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
