using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Command.Precondition;

namespace XayahBot.Command.Account
{
    [Group("account")]
    //[Category(CategoryType.ACCOUNT)]
    public class CAccount : ModuleBase
    {
        [Command("register")]
        [RequireContext(ContextType.DM)]
        public Task Register()
        {
            return Task.CompletedTask;
        }

        [Command("refresh")]
        [RequireContext(ContextType.DM)]
        public Task Refresh()
        {
            return Task.CompletedTask;
        }
    }
}
