using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Error;
using XayahBot.Database.Model;

namespace XayahBot.Database.DAO
{
    public class IncidentSubscriberDAO
    {
        public TIncidentSubscriber GetSingleByChannelId(ulong channelId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncidentSubscriber match = database.IncidentSubscriber.FirstOrDefault(x => x.ChannelId.Equals(channelId));
                return match ?? throw new NotExistingException();
            }
        }

        public TIncidentSubscriber GetSingleByGuildId(ulong guildId)
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

        public async Task SaveAsync(TIncidentSubscriber entry)
        {
            if (this.HasChannelSubscribed(entry.ChannelId))
            {
                throw new AlreadyExistingException();
            }
            else
            {
                await this.AddAsync(entry);
            }
        }

        private async Task AddAsync(TIncidentSubscriber entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                database.IncidentSubscriber.Add(entry);
                if (await database.SaveChangesAsync() <= 0)
                {
                    throw new NotSavedException();
                }
            }
        }

        public async Task RemoveByChannelIdAsync(ulong channelId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncidentSubscriber match = this.GetSingleByChannelId(channelId);
                database.Remove(match);
                if (await database.SaveChangesAsync() <= 0)
                {
                    throw new NotSavedException();
                }
            }
        }

        public async Task RemoveByGuildIdAsync(ulong guildId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncidentSubscriber match = this.GetSingleByGuildId(guildId);
                database.Remove(match);
                if (await database.SaveChangesAsync() <= 0)
                {
                    throw new NotSavedException();
                }
            }
        }

        public bool HasChannelSubscribed(ulong channelId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                if (database.IncidentSubscriber.Where(x => x.ChannelId.Equals(channelId)).Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasGuildSubscribed(ulong guildId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                if (database.IncidentSubscriber.Where(x => x.GuildId.Equals(guildId)).Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasAnySubscriber()
        {
            if (this.GetAll().Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
