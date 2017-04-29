#pragma warning disable 1998

using Discord.Commands;
using System.Threading.Tasks;
using XayahBot.Database.Service;

namespace XayahBot.Command.Attribute
{
    public class CheckIgnoredUserAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (map.Get<IgnoreService>().IsIgnored(context.Guild.Id, context.User.Id))
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
