#pragma warning disable 1998

using System.Threading.Tasks;
using Discord.Commands;

namespace XayahBot.Command.Precondition
{
    public class RequireModAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            string errorText = "You don't have the required permission for this command";

            if (DiscordPermissions.IsOwnerOrMod(context))
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
