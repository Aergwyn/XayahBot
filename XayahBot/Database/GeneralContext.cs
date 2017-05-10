using Microsoft.EntityFrameworkCore;
using XayahBot.Database.Model;
using XayahBot.Utility;

namespace XayahBot.Database
{
    public class GeneralContext : DbContext
    {
        public DbSet<TProperty> Properties { get; set; }
        public DbSet<TLeaderboardEntry> QuizLeaderboard { get; set; }
        public DbSet<TIgnoreEntry> IgnoreList { get; set; }
        public DbSet<TRemindEntry> Reminder { get; set; }
        public DbSet<TIncidentSubscriber> IncidentSubscriber { get; set; }
        public DbSet<TPostedIncident> PostedIncidents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={Property.DbName.Value}");
        }
    }
}
