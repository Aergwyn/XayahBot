using Microsoft.EntityFrameworkCore;
using XayahBot.Database.Model;

namespace XayahBot.Database
{
    public class GeneralContext : DbContext
    {
        public DbSet<TProperty> Properties { get; set; }
        public DbSet<TReminder> Reminder { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=xayah.db");
        }
    }
}
