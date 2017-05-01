using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Error;

namespace XayahBot.Utility
{
    public class ResponseHelper
    {
        public async Task<IMessageChannel> GetDMChannel(CommandContext context)
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

        public async Task<IMessageChannel> GetDMChannel(DiscordSocketClient client, ulong userId)
        {
            IUser user = client.GetUser(userId);
            return await user.CreateDMChannelAsync() ?? throw new NoResponseChannelException();
        }
    }
}
