using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace XayahBot.Extension
{
    public static class IUserMessageExtension
    {
        public static async Task AddReactionIfNotDMAsync(this IUserMessage message, ICommandContext context, IEmote emote)
        {
            if (!(context as CommandContext).IsPrivate)
            {
                await message.AddReactionAsync(emote);
            }
        }
    }
}
