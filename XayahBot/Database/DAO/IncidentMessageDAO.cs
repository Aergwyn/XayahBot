using System.Threading.Tasks;
using XayahBot.Database.Model;

namespace XayahBot.Database.DAO
{
    public class IncidentMessageDAO : AbstractDAO<TIncidentMessage>
    {
        public async Task RemoveRangeAsync(params TIncidentMessage[] incidentMessages)
        {
            using (GeneralContext database = new GeneralContext())
            {
                database.IncidentMessages.RemoveRange(incidentMessages);
                await database.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
