using Microsoft.EntityFrameworkCore;
using XayahBot.Database.Model;
using XayahBot.Utility;

namespace XayahBot.Database
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
