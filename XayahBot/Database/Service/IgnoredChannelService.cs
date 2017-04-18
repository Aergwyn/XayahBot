using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;

namespace XayahBot.Database.Service
{
    public static class IgnoredChannelService
    {
        public static bool IsChannelIgnored(ulong channel)
        {
            using (GeneralContext db = new GeneralContext())
            {
                if (db.IgnoredChannel.FirstOrDefault(x => x.ChannelId.Equals(channel)) != null)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<TIgnoredChannel> GetChannelList()
        {
            using (GeneralContext db = new GeneralContext())
            {
                List<TIgnoredChannel> result = new List<TIgnoredChannel>();
                foreach (TIgnoredChannel channel in db.IgnoredChannel)
                {
                    result.Add(channel);
                }
                return result;
            }
        }

        public static async Task<bool> AddChannelAsync(ulong guild, ulong channelId, string channelName)
        {
            using (GeneralContext db = new GeneralContext())
            {
                TIgnoredChannel match = db.IgnoredChannel.FirstOrDefault(x => x.Guild.Equals(guild) && x.ChannelId.Equals(channelId));
                if (match == null)
                {
                    db.IgnoredChannel.Add(new TIgnoredChannel { Guild = guild, ChannelId = channelId, ChannelName = channelName });
                }
                if (await db.SaveChangesAsync() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static async Task<bool> RemoveChannelAsync(ulong guild, ulong channelId)
        {
            using (GeneralContext db = new GeneralContext())
            {
                TIgnoredChannel match = db.IgnoredChannel.FirstOrDefault(x => x.Guild.Equals(guild) && x.ChannelId.Equals(channelId));
                if (match != null)
                {
                    db.Remove(match);
                }
                if (await db.SaveChangesAsync() > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
