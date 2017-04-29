#pragma warning disable 4014

using System.Threading.Tasks;
using Discord;
using Discord.Commands;
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
    }
}
