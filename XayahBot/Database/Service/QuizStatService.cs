using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Utility;

namespace XayahBot.Database.Service
{
    public static class QuizStatService
    {
        public static List<TQuizStat> GetStatList(ulong guild)
        {
            CheckForReset();
            using (Context db = new Context())
            {
                return db.QuizStats.Where(x => x.Guild.Equals(guild)).ToList();
            }
        }

        public static Task IncrementAnswerAsync(ulong guild, string user)
        {
            CheckForReset();
            using (Context db = new Context())
            {
                TQuizStat quizStat = db.QuizStats.FirstOrDefault(x => x.Guild.Equals(guild) && x.User.Equals(user));
                if (quizStat == null)
                {
                    db.QuizStats.Add(new TQuizStat() { Guild = guild, User = user, Answers = 1 });
                }
                else
                {
                    quizStat.Answers++;
                }
                db.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }

        //

        private static void CheckForReset()
        {
            if (!string.IsNullOrWhiteSpace(Property.QuizLastReset.Value))
            {
                DateTime now = DateTime.UtcNow;
                string[] lastResetValue = Property.QuizLastReset.Value.Split('/');
                DateTime lastReset = new DateTime(int.Parse(lastResetValue.ElementAt(1)), int.Parse(lastResetValue.ElementAt(0)), 1, 0, 0, 0);
                if (lastReset.AddMonths(1) < now)
                {
                    using (Context db = new Context())
                    {
                        foreach (TQuizStat quizStat in db.QuizStats)
                        {
                            db.Remove(quizStat);
                        }
                        db.SaveChangesAsync();
                    }
                    Property.QuizLastReset.Value = $"{now.Month}/{now.Year}";
                }
            }
        }
    }
}
