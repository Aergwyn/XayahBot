using Discord.Commands;
using System.Threading.Tasks;
using XayahBot.Service;

namespace XayahBot.Command.Attribute
{
    public class CheckIgnoredUserAttribute : PreconditionAttribute
    {
#pragma warning disable 1998 // Intentional
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (PermissionService.IsIgnored(context.User))
            {
                return PreconditionResult.FromError("You are on the ignore list for this bot and can't execute this command.");
            }
            else
            {
                return PreconditionResult.FromSuccess();
            }
        }
#pragma warning restore
    }
}
