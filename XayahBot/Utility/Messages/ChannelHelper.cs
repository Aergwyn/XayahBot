using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Error;

namespace XayahBot.Utility.Messages
{
    public static class ChannelHelper
    {
        public static IMessageChannel GetChannel(DiscordSocketClient client, ulong channelId)
        {
            return client.GetChannel(channelId) as IMessageChannel ?? throw new NoResponseChannelException();
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
                channel = await context.Message.Author.CreateDMChannelAsync() ?? throw new NoResponseChannelException();
            }
            return channel;
        }

        public static async Task<IMessageChannel> GetDMChannel(DiscordSocketClient client, ulong userId)
        {
            IUser user = client.GetUser(userId);
            return await user?.CreateDMChannelAsync() ?? throw new NoResponseChannelException();
        }
    }
}
