using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Error;
using XayahBot.Database.Model;

namespace XayahBot.Database.DAO
{
    public class AccountsDAO
    {
        public TAccount GetSingle(ulong userId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TAccount match = database.Accounts.FirstOrDefault(x => x.UserId.Equals(userId));
                return match ?? throw new NotExistingException();
            }
        }

        public List<TAccount> GetAll(ulong userId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                List<TAccount> matches = new List<TAccount>();
                try
                {
                    TAccount match = this.GetSingle(userId);
                    matches.AddRange(database.Accounts.Where(x => x.SummonerId.Equals(match.SummonerId)));
                }
                catch (NotExistingException)
                {
                }
                return matches;
            }
        }

        public async Task SaveAsync(TAccount entry)
        {
            if (this.HasAccount(entry.UserId))
            {
                throw new AlreadyExistingException();
            }
            else
            {
                await this.AddAsync(entry);
            }
        }

        private async Task AddAsync(TAccount entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                database.Accounts.Add(entry);
                if (await database.SaveChangesAsync() <= 0)
                {
                    throw new NotSavedException();
                }
            }
        }

        public async Task RemoveByUserIdAsync(ulong userId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                List<TAccount> matches = this.GetAll(userId);
                if (matches.Count > 0)
                {
                    database.RemoveRange(matches);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
            }
        }

        public bool HasAccount(ulong userId)
        {
            try
            {
                this.GetSingle(userId);
                return true;
            }
            catch (NotExistingException)
            {
                return false;
            }
        }
    }
}
