using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;

namespace XayahBot.Database.Service
{
    public static class IgnoredChannelService
    {
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

        public static Task ToggleChannelAsync(ulong guild, ulong channel)
        {
            using (GeneralContext db = new GeneralContext())
            {
                TIgnoredChannel match = db.IgnoredChannel.FirstOrDefault(x => x.Guild.Equals(guild) && x.Channel.Equals(channel));
                if (match == null)
                {
                    db.IgnoredChannel.Add(new TIgnoredChannel { Guild = guild, Channel = channel });
                }
                else
                {
                    db.Remove(match);
                }
                db.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }
    }
}
