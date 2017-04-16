using Microsoft.EntityFrameworkCore;
using XayahBot.Utility;

namespace XayahBot.Database.Model
{
    public class Context : DbContext
    {
        public DbSet<TProperty> Properties { get; set; }
        public DbSet<TQuizStat> QuizStats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={Property.DbName.Value}");
        }
    }
}
