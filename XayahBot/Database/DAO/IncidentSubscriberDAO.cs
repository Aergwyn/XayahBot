using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;

namespace XayahBot.Database.DAO
{
    public class IncidentSubscriberDAO
    {
        public List<TIncidentSubscriber> GetSubscriber()
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.IncidentSubscriber.ToList();
            }
        }

        public async Task AddAsync(TIncidentSubscriber entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncidentSubscriber match = database.IncidentSubscriber.FirstOrDefault(x => x.GuildId.Equals(entry.GuildId));
                if (match == null)
                {
                    database.IncidentSubscriber.Add(entry);
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

        public async Task RemoveByGuildIdAsync(ulong guildId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncidentSubscriber match = database.IncidentSubscriber.FirstOrDefault(x => x.GuildId.Equals(guildId));
                if (match != null)
                {
                    database.Remove(match);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
                else
                {
                    throw new NotExistingException();
                }
            }
        }

        public async Task RemoveByChannelIdAsync(ulong channelId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncidentSubscriber match = database.IncidentSubscriber.FirstOrDefault(x => x.ChannelId.Equals(channelId));
                if (match != null)
                {
                    database.Remove(match);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
                else
                {
                    throw new NotExistingException();
                }
            }
        }

        public bool HasSubscriber()
        {
            using (GeneralContext database = new GeneralContext())
            {
                if (database.IncidentSubscriber.Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasGuildSubscriber(ulong guildId)
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

        public bool HasChannelSubscriber(ulong channelId)
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
    }
}
