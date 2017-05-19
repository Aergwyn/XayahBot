#pragma warning disable 4014

using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.API.Riot;
using XayahBot.Command.Precondition;
using XayahBot.Database.DAO;
using XayahBot.Database.Error;
using XayahBot.Database.Model;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Account
{

    [Group("account")]
    [Category(CategoryType.ACCOUNT)]
    public class CAccount : ModuleBase
    {
        private readonly RegistrationService _registrationService;
        private readonly AccountsDAO _accountsDao = new AccountsDAO();

        public CAccount(RegistrationService registrationService)
        {
            this._registrationService = registrationService;
        }

        [Command("register")]
        [Summary("Registers your league account to fetch data from Riot API.")]
        public async Task Register([OverrideTypeReader(typeof(RegionTypeReader))]Region region, [Remainder]string summonerName)
        {
            try
            {
                IMessageChannel channel = await ChannelRetriever.GetDMChannelAsync(this.Context);
                this._accountsDao.GetSingle(summonerName.Trim(), region);
                DiscordFormatEmbed message = new DiscordFormatEmbed()
                    .AppendDescription("You are already registered.");
                await channel.SendMessageAsync("", false, message.ToEmbed());
            }
            catch (NotExistingException)
            {
                this._registrationService.DoRegistration(this.Context.User, summonerName.Trim(), region);
            }
        }

        [Command("refresh")]
        [Summary("Updates data of your account.")]
        public Task Refresh([OverrideTypeReader(typeof(RegionTypeReader))]Region region, [Remainder]string summonerName)
        {
            try
            {
                TAccount account = this._accountsDao.GetSingle(summonerName.Trim(), region);
                DiscordFormatEmbed message = new DiscordFormatEmbed()
                    .AppendDescription("If it would've been implemented I could update your data now.");
                this.ReplyAsync("", false, message.ToEmbed());
            }
            catch (NotExistingException)
            {
                // ??? say it's not registered?
            }
            return Task.CompletedTask;
        }

        [Command("unregister")]
        [Summary("Revokes yadayada.")]
        public Task Unregister()
        {
            // remove user from database
            // notify data is going to expire in X and until then still accessible
            return Task.CompletedTask;
        }
    }
}
