using Microsoft.EntityFrameworkCore;
using XayahBot.Database.Model;

namespace XayahBot.Database
{
    public class GeneralContext : DbContext
    {
        public DbSet<TProperty> Properties { get; set; }
        public DbSet<TReminder> Reminder { get; set; }
        public DbSet<TIncidentSubscriber> IncidentSubscriber { get; set; }
        public DbSet<TIncident> Incidents { get; set; }
        public DbSet<TIncidentMessage> IncidentMessages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=xayah.db");
        }
    }
}
