using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Error;
using XayahBot.Database.Model;

namespace XayahBot.Database.DAO
{
    public class ReminderDAO
    {
        public List<TRemindEntry> GetAll(ulong userId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.Reminder.Where(x => x.UserId.Equals(userId)).ToList();
            }
        }

        public List<TRemindEntry> GetAll()
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.Reminder.ToList();
            }
        }

        public async Task SaveAsync(TRemindEntry entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                entry.UserEntryNumber = this.GetEntryNumber(entry.UserId);
                database.Reminder.Add(entry);
                if (await database.SaveChangesAsync() <= 0)
                {
                    throw new NotSavedException();
                }
            }
        }

        public async Task RemoveAsync(ulong userId, int userEntryNumber)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TRemindEntry match = database.Reminder.FirstOrDefault(x => x.UserId.Equals(userId) && x.UserEntryNumber.Equals(userEntryNumber));
                if (match != null)
                {
                    database.Remove(match);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
                else
                {
                    throw new NotExistingException();
                }
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

        private int GetEntryNumber(ulong userId)
        {
            List<TRemindEntry> existingReminder = this.GetAll(userId);
            int userEntryNumber = 0;
            do
            {
                userEntryNumber++;
            }
            while (existingReminder.Where(x => x.UserEntryNumber.Equals(userEntryNumber)).Count() > 0);
            return userEntryNumber;
        }
    }
}
