using Microsoft.EntityFrameworkCore;
using XayahBot.Utility;

namespace XayahBot.Database.Model
{
    public class GeneralContext : DbContext
    {
        public DbSet<TProperty> Properties { get; set; }

        //

        public DbSet<TLeaderboardEntry> Leaderboard { get; set; }
        public DbSet<TIgnoreEntry> IgnoreList { get; set; }

        //

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={Property.DbName.Value}");
        }
    }
}
