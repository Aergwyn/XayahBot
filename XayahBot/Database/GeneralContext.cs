using Microsoft.EntityFrameworkCore;
using XayahBot.Database.Model;
using XayahBot.Utility;

namespace XayahBot.Database
{
    public class GeneralContext : DbContext
    {
        public DbSet<TProperty> Properties { get; set; }
        public DbSet<TIgnoreEntry> IgnoreList { get; set; }
        public DbSet<TRemindEntry> Reminder { get; set; }
        public DbSet<TIncidentSubscriber> IncidentSubscriber { get; set; }
        public DbSet<TIncident> Incidents { get; set; }
        public DbSet<TMessage> Messages { get; set; }
        public DbSet<TAccount> Accounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={Property.DbName.Value}");
        }
    }
}
