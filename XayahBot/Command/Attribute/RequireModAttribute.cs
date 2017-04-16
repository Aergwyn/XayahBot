using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Service;

namespace XayahBot.Command.Attribute
{
    public class RequireModAttribute : PreconditionAttribute
    {
#pragma warning disable 1998 // Intentional
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (PermissionService.IsAdminOrMod(context as CommandContext))
            {
                return PreconditionResult.FromSuccess();
            }
            else
            {
                return PreconditionResult.FromError("You must be a mod (explicitly for this bot) to run this command.");
            }
        }
#pragma warning restore
    }
}
