using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                TIncident match = database.Incidents.FirstOrDefault(x => x.IncidentId.Equals(incidentId));
                if (match != null)
                {
                    database.Entry(match).Collection(x => x.Messages).Load();
                }
                return match ?? throw new NotExistingException();
            }
        }

        public List<TIncident> Get()
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.Incidents.ToList();
            }
        }
    }
}
