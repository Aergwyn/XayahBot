#pragma warning disable 4014

using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.API.Riot;
using XayahBot.Command.Precondition;
using XayahBot.Database.DAO;
using XayahBot.Database.Error;
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
        [Summary("Register yadayada.")]
        public Task Register([OverrideTypeReader(typeof(RegionTypeReader))]Region region, [Remainder]string summonerName)
        {
            this._registrationService.DoRegistration(this.Context.User, summonerName, region);
            return Task.CompletedTask;
        }

        [Command("refresh")]
        [Summary("Update yadayada.")]
        public Task Refresh()
        {
            try
            {
                //TAccount account = this._accountsDao.GetSingle(summonerName.Trim(), region);
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
        [Summary("Revoke yadayada.")]
        public async Task Unregister()
        {
            DiscordFormatEmbed message = new DiscordFormatEmbed();
            try
            {
                await this._accountsDao.RemoveByUserIdAsync(this.Context.User.Id);
                message.AppendDescription("The summoner was successfully unregistered.");
            }
            catch (NotExistingException)
            {
                message.AppendDescription("There was no registered summoner to begin with.");
            }
            this.ReplyAsync("", false, message.ToEmbed());
        }
    }
}
