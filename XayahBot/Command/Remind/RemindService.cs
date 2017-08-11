using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using XayahBot.Database.DAO;
using XayahBot.Database.Model;
using XayahBot.Extension;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Remind
{
    public class RemindService
    {
        private static RemindService _instance;

        public static RemindService GetInstance(IDiscordClient client)
        {
            if (_instance == null)
            {
                _instance = new RemindService(client);
            }
            return _instance;
        }

        // ---

        private readonly IDiscordClient _client;
        private readonly ReminderDAO _reminderDAO = new ReminderDAO();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private Task _process;
        private bool _isRunning = false;
        private Dictionary<string, Timer> _currentTimerList = new Dictionary<string, Timer>();

        private RemindService(IDiscordClient client)
        {
            this._client = client;
        }

        public async Task StartAsync()
        {
            await this._lock.WaitAsync();
            try
            {
                if (!this._isRunning && this._reminderDAO.HasReminder())
                {
                    this._process = Task.Run(() => this.RunAsync());
                    Logger.Info($"{nameof(RemindService)} started.");
                }
            }
            finally
            {
                this._lock.Release();
            }
        }

        private async Task RunAsync()
        {
            bool init = true;
            bool processed = false;
            this._isRunning = true;
            while (this._isRunning)
            {
                int interval = 5;
                if (DateTime.UtcNow.Minute % interval == 0 || init)
                {
                    if (!processed)
                    {
                        init = false;
                        processed = true;
                        List<TReminder> reminder = this._reminderDAO.GetAll();
                        List<TReminder> dueReminder = reminder.Where(x => !this._currentTimerList.Keys.Contains(this.BuildTimerKey(x.UserId, x.ExpirationTime))).ToList();
                        await this.ProcessExpiringReminderAsync(dueReminder, interval);
                    }
                }
                else
                {
                    processed = false;
                }
                if (this._reminderDAO.HasReminder())
                {
                    await Task.Delay(new TimeSpan(0, 0, 10));
                }
                else
                {
#pragma warning disable 4014
                    this.StopAsync();
#pragma warning restore 4014
                }
            }
        }

        private Task ProcessExpiringReminderAsync(List<TReminder> dueReminder, int interval)
        {
            foreach (TReminder reminder in dueReminder)
            {
                if (DateTime.UtcNow.AddMinutes(interval) > reminder.ExpirationTime)
                {
                    long remainingTicks = reminder.ExpirationTime.Ticks - DateTime.UtcNow.Ticks;
                    if (remainingTicks < 0)
                    {
                        remainingTicks = 0;
                    }
                    this._currentTimerList.Add(this.BuildTimerKey(reminder.UserId, reminder.ExpirationTime),
                        new Timer(this.HandleExpiredReminder, reminder, new TimeSpan(remainingTicks), new TimeSpan(Timeout.Infinite)));
                }
            }
            return Task.CompletedTask;
        }

        private string BuildTimerKey(ulong userId, DateTime expirationTime)
        {
            return userId.ToString() + expirationTime.ToString();
        }

        private async void HandleExpiredReminder(object state)
        {
            TReminder reminder = state as TReminder;
            await this._reminderDAO.RemoveAsync(reminder);
            StopTimer(this.BuildTimerKey(reminder.UserId, reminder.ExpirationTime));

            IMessageChannel channel = await ChannelProvider.GetDMChannelAsync(this._client, reminder.UserId);
            FormattedEmbedBuilder message = new FormattedEmbedBuilder()
                .AppendTitle($"{XayahReaction.Clock} Reminder expired")
                .AppendDescription(reminder.Message);
            await channel.SendEmbedAsync(message);
        }

        private void StopTimer(string key)
        {
            if (this._currentTimerList.TryGetValue(key, out Timer timer))
            {
                timer.Dispose();
                this._currentTimerList.Remove(key);
            }
        }

        public async Task StopAsync()
        {
            Logger.Info($"Requested stop for {nameof(RemindService)}.");
            this._isRunning = false;

            foreach (Timer timer in this._currentTimerList.Values)
            {
                timer.Dispose();
            }
            this._currentTimerList.Clear();

            if (this._process != null)
            {
                await this._process;
            }
        }

        public async Task AddNewAsync(TReminder reminder)
        {
            await this._reminderDAO.SaveAsync(reminder);
            await StartAsync();
        }

        public async Task ClearUserAsync(ulong userId)
        {
            await this._reminderDAO.RemoveByUserAsync(userId);
            foreach (KeyValuePair<string, Timer> pair in this._currentTimerList.Where(x => x.Key.StartsWith(userId.ToString())))
            {
                pair.Value.Dispose();
            }
        }
    }
}
