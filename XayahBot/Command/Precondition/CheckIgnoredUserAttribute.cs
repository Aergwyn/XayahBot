using System;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Database.DAO;

namespace XayahBot.Command.Precondition
{
    public class CheckIgnoredUserAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            IgnoreListDAO ignoreListDAO = new IgnoreListDAO();
            if (ignoreListDAO.HasSubject(context.Guild.Id, context.User.Id))
            {
                return Task.FromResult(PreconditionResult.FromError("You are on the ignore list for this bot and can't execute this command"));
            }
            else
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
        }
    }
}
