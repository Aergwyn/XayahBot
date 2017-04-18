using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Service;

namespace XayahBot.Command.Attribute
{
    public class RequireAdminAttribute : PreconditionAttribute
    {
#pragma warning disable 1998 // Intentional
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (PermissionService.IsAdmin(context.User))
            {
                return PreconditionResult.FromSuccess();
            }
            else
            {
                return PreconditionResult.FromError("You need the admin permission (for this bot) to execute this command.");
            }
        }
#pragma warning restore
    }
}
