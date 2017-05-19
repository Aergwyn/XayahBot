using System.Linq;
using System.Threading.Tasks;
using XayahBot.API.Riot;
using XayahBot.Database.Error;
using XayahBot.Database.Model;

namespace XayahBot.Database.DAO
{
    public class AccountsDAO
    {
        public TAccount GetSingle(long accountId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TAccount match = database.Accounts.FirstOrDefault(x => x.SummonerId.Equals(accountId));
                return match ?? throw new NotExistingException();
            }
        }

        public TAccount GetSingle(string name, Region region)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TAccount match = database.Accounts.FirstOrDefault(x => x.Region.Equals(region.Name) && x.Name.Equals(name));
                return match ?? throw new NotExistingException();
            }
        }

        public async Task SaveAsync(TAccount entry)
        {
            if (this.HasAccount(entry.SummonerId))
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

        public bool HasAccount(long accountId)
        {
            try
            {
                this.GetSingle(accountId);
                return true;
            }
            catch (NotExistingException)
            {
                return false;
            }
        }
    }
}
