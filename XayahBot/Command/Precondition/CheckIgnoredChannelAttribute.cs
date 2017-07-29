using System;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Database.DAO;

namespace XayahBot.Command.Precondition
{
    public class CheckIgnoredChannelAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            IgnoreListDAO ignoreListDAO = new IgnoreListDAO();
            if (ignoreListDAO.HasSubject(context.Guild.Id, context.Channel.Id))
            {
                return Task.FromResult(PreconditionResult.FromError("This channel is on the ignore list for this bot and can't accept some commands"));
            }
            else
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
        }
    }
}
