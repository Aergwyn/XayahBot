using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Error;

namespace XayahBot.Utility
{
    public static class ChannelRetriever
    {
        public static IMessageChannel GetChannel(DiscordSocketClient client, ulong channelId)
        {
            return client.GetChannel(channelId) as IMessageChannel ?? throw new NoChannelException();
        }

        public static async Task<IMessageChannel> GetDMChannel(CommandContext context)
        {
            IMessageChannel channel = null;
            if (context.IsPrivate)
            {
                channel = context.Channel;
            }
            else
            {
                channel = await context.Message.Author.CreateDMChannelAsync().ConfigureAwait(false);
            }
            return channel ?? throw new NoChannelException();
        }

        public static async Task<IMessageChannel> GetDMChannel(DiscordSocketClient client, ulong userId)
        {
            IUser user = client.GetUser(userId);
            IMessageChannel channel = await user?.CreateDMChannelAsync(); // Why can't I ConfigureAwait(false) here?
            return channel ?? throw new NoChannelException();
        }
    }
}
