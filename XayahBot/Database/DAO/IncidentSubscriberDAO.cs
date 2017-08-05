using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;

namespace XayahBot.Database.DAO
{
    public class IncidentSubscriberDAO : AbstractDAO<TIncidentSubscriber>
    {
        public TIncidentSubscriber GetByChannelId(ulong channelId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncidentSubscriber match = database.IncidentSubscriber.FirstOrDefault(x => x.ChannelId.Equals(channelId));
                return match ?? throw new NotExistingException();
            }
        }

        public TIncidentSubscriber GetByGuildId(ulong guildId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncidentSubscriber match = database.IncidentSubscriber.FirstOrDefault(x => x.GuildId.Equals(guildId));
                return match ?? throw new NotExistingException();
            }
        }

        public List<TIncidentSubscriber> GetAll()
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.IncidentSubscriber.ToList();
            }
        }

        public async Task RemoveByChannelIdAsync(ulong channelId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                try
                {
                    TIncidentSubscriber match = this.GetByChannelId(channelId);
                    await this.RemoveAsync(match);
                }
                catch (NotExistingException)
                {
                }
            }
        }

        public async Task RemoveByGuildIdAsync(ulong guildId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                try
                {
                    TIncidentSubscriber match = this.GetByGuildId(guildId);
                    await this.RemoveAsync(match);
                }
                catch (NotExistingException)
                {
                }
            }
        }

        public bool HasSubscriber()
        {
            if (this.GetAll().Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
