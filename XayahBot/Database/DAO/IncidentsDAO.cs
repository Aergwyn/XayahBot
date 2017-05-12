using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;

namespace XayahBot.Database.DAO
{
    public class IncidentsDAO
    {
        public List<TIncident> GetIncidents()
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.Incidents.ToList();
            }
        }

        public TIncident GetIncident(long incidentId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncident match = database.Incidents.FirstOrDefault(x => x.IncidentId.Equals(incidentId));
                if (match != null)
                {
                    database.Entry(match).Collection(x => x.Messages).Load();
                    return match;
                }
            }
            throw new NotExistingException();
        }

        public async Task SaveAsync(TIncident entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncident match = database.Incidents.FirstOrDefault(x => x.IncidentId.Equals(entry.IncidentId));
                if (match == null)
                {
                    await this.AddAsync(entry);
                }
                else
                {
                    await this.UpdateAsync(entry);
                }
            }
        }

        private async Task AddAsync(TIncident entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncident match = database.Incidents.FirstOrDefault(x => x.IncidentId.Equals(entry.IncidentId));
                if (match == null)
                {
                    database.Incidents.Add(entry);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
                else
                {
                    throw new AlreadyExistingException();
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
                this.GetIncident(incidentId);
                return true;
            }
            catch (NotExistingException)
            {
                return false;
            }
        }
    }
}
