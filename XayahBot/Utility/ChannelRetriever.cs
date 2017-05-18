using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Error;

namespace XayahBot.Utility
{
    public static class ChannelRetriever
    {
        public static async Task<IMessageChannel> GetDMChannelAsync(CommandContext context)
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

        public static async Task<IMessageChannel> GetDMChannelAsync(DiscordSocketClient client, ulong userId)
        {
            IUser user = client.GetUser(userId);
            IMessageChannel channel = await user?.CreateDMChannelAsync();
            return channel ?? throw new NoChannelException();
        }
    }
}
