using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;
using XayahBot.Utility;

namespace XayahBot.Database.DAO
{
    public class QuizLeaderboardDAO
    {
        public TLeaderboardEntry GetSingle(ulong guildId, ulong userId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TLeaderboardEntry match = database.QuizLeaderboard.FirstOrDefault(x => x.GuildId.Equals(guildId) && x.UserId.Equals(userId));
                return match ?? throw new NotExistingException();
            }
        }

        public async Task<List<TLeaderboardEntry>> GetAllAsync(ulong guildId)
        {
            await this.ResetAsync();
            using (GeneralContext database = new GeneralContext())
            {
                return database.QuizLeaderboard.Where(x => x.GuildId.Equals(guildId)).ToList();
            }
        }

        public async Task IncrementAnswerAsync(ulong guildId, ulong userId)
        {
            await this.ResetAsync();
            using (GeneralContext database = new GeneralContext())
            {
                try
                {
                    TLeaderboardEntry match = this.GetSingle(guildId, userId);
                    match.Answers++;
                }
                catch (NotExistingException)
                {
                    TLeaderboardEntry entry = new TLeaderboardEntry
                    {
                        Answers = 1,
                        GuildId = guildId,
                        UserId = userId
                    };
                    database.QuizLeaderboard.Add(entry);
                }
                if (await database.SaveChangesAsync() <= 0)
                {
                    throw new NotSavedException();
                }
            }
        }

        private async Task ResetAsync()
        {
            if (this.IsResetNeeded())
            {
                using (GeneralContext database = new GeneralContext())
                {
                    if (database.QuizLeaderboard.Count() > 0)
                    {
                        database.QuizLeaderboard.RemoveRange(database.QuizLeaderboard);
                        if (await database.SaveChangesAsync() <= 0)
                        {
                            throw new NotSavedException();
                        }
                    }
                    DateTime now = DateTime.UtcNow;
                    Property.QuizLastReset.Value = $"{now.Month}/{now.Year}";
                }
            }
        }

        private bool IsResetNeeded()
        {
            if (string.IsNullOrWhiteSpace(Property.QuizLastReset.Value))
            {
                return true;
            }
            DateTime now = DateTime.UtcNow;
            string[] lastReset = Property.QuizLastReset.Value.Split('/');
            if (new DateTime(int.Parse(lastReset.ElementAt(1)), int.Parse(lastReset.ElementAt(0)), 1, 0, 0, 0).AddMonths(1) < now)
            {
                return true;
            }
            return false;
        }
    }
}
