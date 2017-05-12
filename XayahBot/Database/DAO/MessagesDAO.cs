using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;

namespace XayahBot.Database.DAO
{
    public class MessagesDAO
    {
        public async Task RemoveByIncidentIdAsync(TIncident incident)
        {
            using (GeneralContext database = new GeneralContext())
            {
                List<TMessage> matches = database.Messages.Where(x => x.Incident.Id.Equals(incident.Id)).ToList();
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
