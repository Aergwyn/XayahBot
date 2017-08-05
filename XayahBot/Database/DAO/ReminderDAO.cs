using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;

namespace XayahBot.Database.DAO
{
    public class ReminderDAO : AbstractDAO<TReminder>
    {
        public List<TReminder> GetAll()
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.Reminder.ToList();
            }
        }

        public List<TReminder> GetAll(ulong userId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.Reminder.Where(x => x.UserId.Equals(userId)).ToList();
            }
        }

        public async Task RemoveByUserAsync(ulong userId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                database.Reminder.RemoveRange(database.Reminder.Where(x => x.UserId.Equals(userId)));
                await database.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public bool HasReminder()
        {
            if (this.GetAll().Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
