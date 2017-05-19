using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.API.Riot;
using XayahBot.Command.Precondition;
using XayahBot.Utility;
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
            IMessageChannel channel = await ChannelRetriever.GetDMChannelAsync(this.Context);
            string code = this._registrationService.NewRegistrant(summonerName, region);
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .AppendDescription($"Rename one of your mastery pages to the following code `{code}` and then use the `account refresh` command.")
                .AppendDescription(Environment.NewLine + Environment.NewLine)
                .AppendDescription("Please remember to do this quick as the code is rendered invalid in a few minutes.");
            await channel.SendMessageAsync("", false, message.ToEmbed());
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
            bool success = this._registrationService.ValidateCode(summonerName, region);
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .AppendDescription($"TEST: {success}");
            this.ReplyAsync("", false, message.ToEmbed());
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
