using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;

namespace XayahBot.Database.DAO
{
    public class IncidentsDAO
    {
        public TIncident GetSingle(long incidentId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncident match = database.Incidents.FirstOrDefault(x => x.IncidentId.Equals(incidentId));
                if (match != null)
                {
                    database.Entry(match).Collection(x => x.Messages).Load();
                }
                return match ?? throw new NotExistingException();
            }
        }

        public List<TIncident> GetAll()
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.Incidents.ToList();
            }
        }

        public async Task SaveAsync(TIncident entry)
        {
            if (this.HasIncident(entry.IncidentId))
            {
                await this.UpdateAsync(entry);
            }
            else
            {
                await this.AddAsync(entry);
            }
        }

        private async Task AddAsync(TIncident entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                database.Incidents.Add(entry);
                if (await database.SaveChangesAsync() <= 0)
                {
                    throw new NotSavedException();
                }
            }
        }

        private async Task UpdateAsync(TIncident entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                database.Update(entry);
                if (await database.SaveChangesAsync() <= 0)
                {
                    throw new NotSavedException();
                }
            }
        }

        public async Task RemoveByIncidentIdAsync(long incidentId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                List<TIncident> matches = database.Incidents.Where(x => x.IncidentId.Equals(incidentId)).ToList();
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

        public bool HasIncident(long incidentId)
        {
            try
            {
                this.GetSingle(incidentId);
                return true;
            }
            catch (NotExistingException)
            {
                return false;
            }
        }
    }
}
