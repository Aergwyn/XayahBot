#pragma warning disable 1998

using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Database.Service;

namespace XayahBot.Command.System
{
    public class CheckIgnoredChannelAttribute : PreconditionAttribute
    {
        private readonly string _errorText = "This channel is on the ignore list for this bot and can't accept some commands";

        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (map.Get<IgnoreDAO>().IsIgnored(context.Guild.Id, context.Channel.Id))
            {
                return PreconditionResult.FromError(this._errorText);
            }
            else
            {
                return PreconditionResult.FromSuccess();
            }
        }
    }
}
