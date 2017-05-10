using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;

namespace XayahBot.Database.DAO
{
    public class RemindDAO
    {
        public List<TRemindEntry> GetReminders()
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.Reminder.ToList();
            }
        }

        public List<TRemindEntry> GetReminders(ulong userId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.Reminder.Where(x => x.UserId.Equals(userId)).ToList();
            }
        }

        public async Task AddAsync(TRemindEntry entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                List<TRemindEntry> existingReminder = this.GetReminders(entry.UserId);
                int newUserEntryNumber = 1;
                while (true)
                {
                    if (existingReminder.Where(x => x.UserEntryNumber.Equals(newUserEntryNumber)).Count() == 0)
                    {
                        break;
                    }
                    newUserEntryNumber++;
                }
                entry.UserEntryNumber = newUserEntryNumber;
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
            if (this.GetReminders().Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
