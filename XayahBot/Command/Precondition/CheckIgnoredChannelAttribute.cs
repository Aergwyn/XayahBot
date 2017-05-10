#pragma warning disable 1998

using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Database.DAO;

namespace XayahBot.Command.Precondition
{
    public class CheckIgnoredChannelAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (map.Get<IgnoreListDAO>().IsIgnored(context.Guild.Id, context.Channel.Id))
            {
                return PreconditionResult.FromError("This channel is on the ignore list for this bot and can't accept some commands");
            }
            else
            {
                return PreconditionResult.FromSuccess();
            }
        }
    }
}
