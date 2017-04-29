#pragma warning disable 1998

using System.Threading.Tasks;
using Discord.Commands;

namespace XayahBot.Command.System
{
    public class RequireModAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            Permission permission = new Permission();
            string errorText = "You don't have the required permission for this command";

            if (permission.IsOwnerOrMod(context))
            {
                return PreconditionResult.FromSuccess();
            }
            else
            {
                return PreconditionResult.FromError(errorText);
            }
        }
    }
}
