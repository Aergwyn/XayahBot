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
                    Logger.Info($"{nameof(RegistrationService)} started.");
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
            Logger.Info($"Requested stop for {nameof(RegistrationService)}.");
            this._isRunning = false;
            if (this._process != null)
            {
                await this._process;
            }
        }

        public async Task DoRegistration(IUser user, string name, Region region)
        {
            RegistrationUser regUser = new RegistrationUser
            {
                Name = name.Trim().ToLower(),
                Region = region,
                UserId = user.Id
            };
            await this._lock.WaitAsync();
            try
            {
                if (this._accountsDao.HasAccount(regUser.UserId))
                {
                    await this.PostDMResponse(regUser, "You already have a summoner registered.");
                }
                else if (!RiotApiUtil.IsValidName(name))
                {
                    await this.PostDMResponse(regUser, $"The summoner name `{regUser.Name}` is not in a valid format.");
                }
                else if (this.IsNewUser(regUser))
                {
                    await this.StartRegistering(regUser);
                }
                else
                {
                    await this.FinishRegistering(regUser);
                }
            }
            finally
            {
                this._lock.Release();
            }
        }

        private async Task PostDMResponse(RegistrationUser user, string text)
        {
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .AppendDescription(text);
            await this.PostDMResponse(user, message);
        }

        private async Task PostDMResponse(RegistrationUser user, DiscordFormatEmbed message)
        {
            IMessageChannel channel = await ChannelProvider.GetDMChannelAsync(this._client, user.UserId);
            await channel.SendMessageAsync("", false, message.ToEmbed());
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
            return this._openRegistrations.FirstOrDefault(x => x.UserId.Equals(user.UserId) && x.Name.Equals(user.Name) && x.Region.Equals(user.Region));
        }

        private async Task StartRegistering(RegistrationUser user)
        {
            user.Code = this.GetGuid();
            this._openRegistrations.Add(user);
            this.StartAsync();
            await this.NotifyOfCode(user);
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

        private async Task NotifyOfCode(RegistrationUser user)
        {
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .AppendDescription("Rename one of your mastery pages to the following registration code ")
                .AppendDescription($"`{user.Code}`", AppendOption.Underscore)
                .AppendDescription(" and then use this command again.")
                .AppendDescription(Environment.NewLine + Environment.NewLine)
                .AppendDescription("Please remember to do this quick as the code is rendered invalid in a few minutes.", AppendOption.Italic);
            await this.PostDMResponse(user, message);
        }

        private async Task FinishRegistering(RegistrationUser user)
        {
            RegistrationUser match = this.GetUser(user);
            if (match.IsExpired())
            {
                await this.PostDMResponse(user, "Your registration code timed out.");
            }
            else
            {
                try
                {
                    if (await this.ValidateCode(match))
                    {
                        await this.AddToDatabase(user);
                        await this.PostDMResponse(user, "You successfully registered your summoner.");
                    }
                    else
                    {
                        await this.PostDMResponse(user, "No mastery page name matches the registration code.");
                    }
                }
                catch (ErrorResponseException)
                {
                    await this.PostDMResponse(user, "There seems to be a problem communicating with the API.");
                }
            }
        }

        private async Task<bool> ValidateCode(RegistrationUser user)
        {
            SummonerApi summonerApi = new SummonerApi(user.Region);
            MasteriesApi masteriesApi = new MasteriesApi(user.Region);
            SummonerDto summoner = await summonerApi.GetSummonerByNameAsync(user.Name);
            MasteryPagesDto masteryPages = await masteriesApi.GetMasteriesBySummonerIdAsync(summoner.Id);
            foreach (MasteryPageDto page in masteryPages.Pages)
            {
                if (page.Name.Equals(user.Code))
                {
                    user.SummonerId = summoner.Id;
                    return true;
                }
            }
            return false;
        }

        private async Task AddToDatabase(RegistrationUser user)
        {
            TAccount entry = new TAccount
            {
                Name = user.Name,
                Region = user.Region.Name,
                SummonerId = user.SummonerId,
                UserId = user.UserId
            };
            await this._accountsDao.SaveAsync(entry);
            this._openRegistrations.Remove(user);
        }
    }
}
