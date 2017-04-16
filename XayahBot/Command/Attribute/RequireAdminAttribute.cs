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
            if (PermissionService.IsAdmin(context as CommandContext))
            {
                return PreconditionResult.FromSuccess();
            }
            else
            {
                return PreconditionResult.FromError("You must be an admin (explicitly for this bot) to run this command.");
            }
        }
#pragma warning restore
    }
}
