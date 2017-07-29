using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace XayahBot.Command.Precondition
{
    public class RequireModAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            string errorText = "You don't have the required permission for this command";

            if (DiscordPermissions.IsOwnerOrMod(context))
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            else
            {
                return Task.FromResult(PreconditionResult.FromError(errorText));
            }
        }
    }
}
