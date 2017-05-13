using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;

namespace XayahBot.Database.DAO
{
    public class MessagesDAO
    {
        public List<TMessage> GetAll(long incidentId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.Messages.Where(x => x.Incident.Id.Equals(incidentId)).ToList();
            }
        }

        public async Task RemoveByIncidentIdAsync(long incidentId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                List<TMessage> matches = this.GetAll(incidentId);
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
    }
}
