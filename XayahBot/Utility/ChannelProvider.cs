using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Error;

namespace XayahBot.Utility
{
    public static class ChannelProvider
    {
        public static async Task<IMessageChannel> GetDMChannelAsync(ICommandContext context)
        {
            return await GetDMChannelAsync(context as CommandContext).ConfigureAwait(false);
        }

        public static async Task<IMessageChannel> GetDMChannelAsync(CommandContext context)
        {
            IMessageChannel channel = null;
            if (context.IsPrivate)
            {
                channel = context.Channel;
            }
            else
            {
                channel = await context.Message.Author.GetOrCreateDMChannelAsync().ConfigureAwait(false);
            }
            return channel ?? throw new NoChannelException();
        }

        public static async Task<IMessageChannel> GetDMChannelAsync(IDiscordClient client, ulong userId)
        {
            IUser user = await client.GetUserAsync(userId);
            IMessageChannel channel = await user?.GetOrCreateDMChannelAsync();
            return channel ?? throw new NoChannelException();
        }
    }
}
