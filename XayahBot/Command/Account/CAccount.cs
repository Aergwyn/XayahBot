using System;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.API.Riot;
using XayahBot.Command.Precondition;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Account
{
    [Group("account")]
    [Category(CategoryType.ACCOUNT)]
    public class CAccount : ModuleBase
    {
        private readonly RegistrationService _registrationService = RegistrationService.GetInstance();

        [Command("register")]
        public async Task Register([OverrideTypeReader(typeof(RegionTypeReader))]Region region, [Remainder]string summonerName)
        {
            string code = this._registrationService.NewRegistrant(summonerName, region);
            DiscordFormatEmbed message = new DiscordFormatEmbed();
            message.AppendDescription($"Rename one of your mastery pages to the following code \"{code}\" and then use the refresh command.")
                .AppendDescription(Environment.NewLine)
                .AppendDescription("Please remember to do this quick as the code is rendered invalid in a few minutes.");
            await this.ReplyAsync("", false, message.ToEmbed());
        }

        [Command("refresh")]
        public Task Refresh([OverrideTypeReader(typeof(RegionTypeReader))]Region region, [Remainder]string summonerName)
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
