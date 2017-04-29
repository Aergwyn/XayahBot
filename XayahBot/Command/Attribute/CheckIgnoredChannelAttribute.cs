#pragma warning disable 1998

using Discord.Commands;
using System.Threading.Tasks;
using XayahBot.Database.Service;

namespace XayahBot.Command.Attribute
{
    public class CheckIgnoredChannelAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (map.Get<IgnoreService>().IsIgnored(context.Guild.Id, context.Channel.Id))
            {
                return PreconditionResult.FromError("This channel is on the ignore list for this bot and can't accept some commands.");
            }
            else
            {
                return PreconditionResult.FromSuccess();
            }
        }
    }
}
