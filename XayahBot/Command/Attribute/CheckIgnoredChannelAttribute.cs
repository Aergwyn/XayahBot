using Discord.Commands;
using System.Threading.Tasks;
using XayahBot.Database.Service;

namespace XayahBot.Command.Attribute
{
    public class CheckIgnoredChannelAttribute : PreconditionAttribute
    {
#pragma warning disable 1998 // Intentional
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (IgnoreService.IsIgnored(context.Guild.Id, context.Channel.Id))
            {
                return PreconditionResult.FromError("This channel is on the ignore list for this bot and can't accept some commands.");
            }
            else
            {
                return PreconditionResult.FromSuccess();
            }
        }
#pragma warning restore
    }
}
