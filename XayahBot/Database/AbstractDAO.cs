using System.Threading.Tasks;

namespace XayahBot.Database
{
    public abstract class AbstractDAO<T> where T : class, IIdentifiable
    {
        public async Task SaveAsync(T entry)
        {
            if (entry.IsNew())
            {
                await this.AddAsync(entry).ConfigureAwait(false);
            }
            else
            {
                await this.UpdateAsync(entry).ConfigureAwait(false);
            }
        }

        protected async Task AddAsync(T entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                database.Add(entry);
                await database.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        protected async Task UpdateAsync(T entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                database.Update(entry);
                await database.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task RemoveAsync(T entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                database.Remove(entry);
                await database.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
