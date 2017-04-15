using Microsoft.EntityFrameworkCore;
using XayahBot.Utility;

namespace XayahBot.Model
{
    public class Context : DbContext
    {
        public DbSet<DbProperty> Properties { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={Property.DbName.Value}");
        }
    }
}
