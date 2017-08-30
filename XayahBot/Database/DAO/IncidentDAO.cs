using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using XayahBot.API.Riot;
using XayahBot.Database.Model;
using XayahBot.Error;

namespace XayahBot.Database.DAO
{
    public class IncidentDAO : AbstractDAO<TIncident>
    {
        public TIncident Get(long incidentId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncident match = database.Incidents.Include(x => x.Messages).FirstOrDefault(x => x.IncidentId.Equals(incidentId));
                if (match != null)
                {
                    database.Entry(match).Collection(x => x.Messages).Load();
                }
                return match ?? throw new NotExistingException();
            }
        }

        public List<TIncident> Get(Region region)
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.Incidents.Include(x => x.Messages).Where(x => x.Region.Equals(region.Name)).ToList();
            }
        }
    }
}
