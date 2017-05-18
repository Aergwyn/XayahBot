using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Command.Precondition;

namespace XayahBot.Command.Account
{
    [Group("account")]
    [Category(CategoryType.ACCOUNT)]
    public class CAccount : ModuleBase
    {
        [Command("register")]
        public Task Register(string summonerName, string region)
        {
            // create random code
            // send to user asking to change random mastery page to that name
            // should refresh if done
            return Task.CompletedTask;
        }

        [Command("refresh")]
        public Task Refresh(string summonerName, string region)
        {
            // get user from database
            // if registered
            //      update cache data
            // if not registered
            //      check mastery page against code
            //      if code correct
            //            update cache data
            return Task.CompletedTask;
        }

        [Command("unregister")]
        public Task Unregister()
        {
            // remove user from database
            // notify data is going to expire in X and until then still accessible
            return Task.CompletedTask;
        }
    }
}
