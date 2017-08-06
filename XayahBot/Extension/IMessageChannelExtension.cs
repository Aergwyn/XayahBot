using System.Threading.Tasks;
using Discord;
using XayahBot.Utility.Messages;

namespace XayahBot.Extension
{
    public static class IMessageChannelExtension
    {
        public static Task<IUserMessage> SendEmbedAsync(this IMessageChannel channel, FormattedEmbedBuilder embedBuilder)
        {
            return channel.SendMessageAsync("", embed: embedBuilder.ToEmbed());
        }
    }
}
