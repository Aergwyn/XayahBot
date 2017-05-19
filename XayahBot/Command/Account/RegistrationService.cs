#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using XayahBot.API.Error;
using XayahBot.API.Riot;
using XayahBot.API.Riot.Model;
using XayahBot.Database.DAO;
using XayahBot.Database.Error;
using XayahBot.Database.Model;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Account
{
    public class RegistrationService
    {
        private static RegistrationService _instance;

        public static RegistrationService GetInstance(IDiscordClient client)
        {
            if (_instance == null)
            {
                _instance = new RegistrationService(client);
            }
            return _instance;
        }

        // ---

        private readonly IDiscordClient _client;
        private readonly AccountsDAO _accountsDao = new AccountsDAO();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private Task _process;
        private bool _isRunning = false;
        private List<RegistrationUser> _openRegistrations = new List<RegistrationUser>();

        private RegistrationService(IDiscordClient client)
        {
            this._client = client;
        }

        public async Task StartAsync()
        {
            await this._lock.WaitAsync();
            try
            {
                if (!this._isRunning && this._openRegistrations.Count > 0)
                {
                    this._process = Task.Run(() => this.RunAsync());
                    Logger.Info("RegistrationService (Purge) started.");
                }
            }
            finally
            {
                this._lock.Release();
            }
        }

        private async Task RunAsync()
        {
            this._isRunning = true;
            while (this._isRunning)
            {
                this._openRegistrations.RemoveAll(x => x.IsExpired());
                if (this._openRegistrations.Count > 0)
                {
                    await Task.Delay(new TimeSpan(0, 0, 10));
                }
                else
                {
                    this.StopAsync();
                }
            }
        }

        public async Task StopAsync()
        {
            this._isRunning = false;
            if (this._process != null)
            {
                await this._process;
                Logger.Info("RegistrationService (Purge) stopped.");
            }
        }

        public async Task DoRegistration(IUser user, string name, Region region)
        {
            RegistrationUser regUser = new RegistrationUser
            {
                Code = this.GetGuid(),
                Name = name.ToLower(),
                Region = region,
                RequestingUserId = user.Id
            };
            if (RiotApiUtil.IsValidName(name))
            {
                if (this.IsNewUser(regUser))
                {
                    await this.StartRegisterProcess(regUser);
                }
                else
                {
                    await this.FinishRegisterProcess(regUser);
                }
            }
            else
            {
                DiscordFormatEmbed message = new DiscordFormatEmbed()
                    .AppendDescription($"The name `{regUser.Name}` is not in a valid format.");
                await this.PostDMResponse(regUser, message);
            }
        }

        private string GetGuid()
        {
            int length = 16;
            string code = Guid.NewGuid().ToString();
            code = code.Replace("-", string.Empty);
            if (code.Length > length)
            {
                code = code.Substring(0, length);
            }
            return code;
        }

        private bool IsNewUser(RegistrationUser user)
        {
            if (this.GetUser(user) == null)
            {
                return true;
            }
            return false;
        }

        private RegistrationUser GetUser(RegistrationUser user)
        {
            return this._openRegistrations.FirstOrDefault(x => x.Name.Equals(user.Name) && x.Region.Equals(user.Region) && x.RequestingUserId.Equals(user.RequestingUserId));
        }

        private async Task StartRegisterProcess(RegistrationUser user)
        {
            this._openRegistrations.Add(user);
            this.StartAsync();
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .AppendDescription($"Rename one of your mastery pages to the following code `{user.Code}` and then use this command again.")
                .AppendDescription(Environment.NewLine + Environment.NewLine)
                .AppendDescription("Please remember to do this quick as the code is rendered invalid in a few minutes.");
            await this.PostDMResponse(user, message);
        }

        private async Task PostDMResponse(RegistrationUser user, DiscordFormatEmbed message)
        {
            IMessageChannel channel = await ChannelProvider.GetDMChannelAsync(this._client, user.RequestingUserId);
            await channel.SendMessageAsync("", false, message.ToEmbed());
        }

        private async Task FinishRegisterProcess(RegistrationUser user)
        {
            RegistrationUser match = this.GetUser(user);
            if (match.IsExpired())
            {
                DiscordFormatEmbed message = new DiscordFormatEmbed()
                    .AppendDescription("Your registration timed out.");
                await this.PostDMResponse(user, message);
            }
            else
            {
                DiscordFormatEmbed message = new DiscordFormatEmbed();
                try
                {
                    SummonerApi summonerApi = new SummonerApi(match.Region);
                    MasteriesApi masteriesApi = new MasteriesApi(match.Region);
                    SummonerDto summoner = await summonerApi.GetSummonerByNameAsync(match.Name);
                    MasteryPagesDto masteryPages = await masteriesApi.GetMasteriesBySummonerIdAsync(summoner.Id);
                    foreach (MasteryPageDto page in masteryPages.Pages)
                    {
                        if (page.Name.Equals(match.Code))
                        {
                            user.SummonerId = summoner.Id;
                            await this.RegistrationSuccess(user);
                            return;
                        }
                    }
                    message.AppendDescription("No mastery page name matches the registration code. Please try again once done.");
                }
                catch (ErrorResponseException)
                {
                    message.AppendDescription("There seems to be a problem communicating with the API.");
                }
                this.PostDMResponse(user, message);
            }
        }

        private async Task RegistrationSuccess(RegistrationUser user)
        {
            DiscordFormatEmbed message = new DiscordFormatEmbed();
            try
            {
                TAccount entry = new TAccount
                {
                    Name = user.Name,
                    Region = user.Region.Name,
                    SummonerId = user.SummonerId
                };
                await this._accountsDao.SaveAsync(entry);
                this._openRegistrations.Remove(user);
                message.AppendDescription("You successfully completed the registration.");
            }
            catch (AlreadyExistingException)
            {
                message.AppendDescription("You are already registered.");
                this.PostDMResponse(user, message);
            }
            await this.PostDMResponse(user, message);
        }
    }
}
