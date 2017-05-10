#pragma warning disable 1998

using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Database.DAO;

namespace XayahBot.Command.Precondition
{
    public class CheckIgnoredUserAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (map.Get<IgnoreListDAO>().IsIgnored(context.Guild.Id, context.User.Id))
            {
                return PreconditionResult.FromError("You are on the ignore list for this bot and can't execute this command");
            }
            else
            {
                return PreconditionResult.FromSuccess();
            }
        }
    }
}
