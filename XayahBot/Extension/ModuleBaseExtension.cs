using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Utility.Messages;

namespace XayahBot.Extension
{
    public static class ModuleBaseExtension
    {
        public static Task<IUserMessage> ReplyAsync(this ModuleBase<ICommandContext> module, FormattedEmbedBuilder embedBuilder)
        {
            return module.Context.Channel.SendEmbedAsync(embedBuilder);
        }
    }
}
