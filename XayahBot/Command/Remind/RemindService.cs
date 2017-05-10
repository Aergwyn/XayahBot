#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using XayahBot.Database.DAO;
using XayahBot.Database.Model;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Remind
{
    public class RemindService
    {
        private static RemindService _instance;

        public static RemindService GetInstance(DiscordSocketClient client)
        {
            if (_instance == null)
            {
                _instance = new RemindService(client);
            }
            return _instance;
        }

        // ---

        private readonly DiscordSocketClient _client;
        private readonly ReminderDAO _reminderDao = new ReminderDAO();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private bool _isRunning = false;
        private Dictionary<string, Timer> _currentTimerList = new Dictionary<string, Timer>();

        private RemindService(DiscordSocketClient client)
        {
            this._client = client;
        }

        public async Task StartAsync()
        {
            await this._lock.WaitAsync();
            try
            {
                if (!this._isRunning && this._reminderDao.HasReminder())
                {
                    this._isRunning = true;
                    Task.Run(() => Run());
                    Logger.Info("ReminderService started.");
                }
            }
            finally
            {
                this._lock.Release();
            }
        }

        private async Task Run()
        {
            try
            {
                bool init = true;
                bool processed = false;
                while (this._isRunning)
                {
                    int interval = 5;
                    if (DateTime.UtcNow.Minute % interval == 0 || init)
                    {
                        if (!processed)
                        {
                            init = false;
                            processed = true;
                            List<TRemindEntry> reminder = this._reminderDao.GetReminder();
                            List<TRemindEntry> dueReminder = reminder.Where(x => !this._currentTimerList.Keys.Contains(this.BuildTimerKey(x.UserId, x.UserEntryNumber))).ToList();
                            await ProcessExpiringReminders(dueReminder, interval);
                        }
                    }
                    else
                    {
                        processed = false;
                    }
                    if (!this._reminderDao.HasReminder())
                    {
                        this._isRunning = false;
                    }
                    await Task.Delay(10000);
                }
            }
            finally
            {
                await this.StopAsync();
            }
        }

        private Task ProcessExpiringReminders(List<TRemindEntry> list, int interval)
        {
            foreach (TRemindEntry reminder in list)
            {
                if (DateTime.UtcNow.AddMinutes(interval) > reminder.ExpirationTime)
                {
                    long remainingTicks = reminder.ExpirationTime.Ticks - DateTime.UtcNow.Ticks;
                    if (remainingTicks < 0)
                    {
                        remainingTicks = 0;
                    }
                    this._currentTimerList.Add(this.BuildTimerKey(reminder.UserId, reminder.UserEntryNumber),
                        new Timer(this.HandleExpiredReminder, reminder, new TimeSpan(remainingTicks), new TimeSpan(Timeout.Infinite)));
                }
            }
            return Task.CompletedTask;
        }

        private string BuildTimerKey(ulong userId, int userEntryNumber)
        {
            return userId.ToString() + userEntryNumber;
        }

        private async void HandleExpiredReminder(object state)
        {
            TRemindEntry reminder = (TRemindEntry)state;
            await this._reminderDao.RemoveAsync(reminder.UserId, reminder.UserEntryNumber);
            StopTimer(this.BuildTimerKey(reminder.UserId, reminder.UserEntryNumber));
            IMessageChannel channel = await ResponseHelper.GetDMChannel(this._client, reminder.UserId);
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .AppendDescription("Back then you told me to remind you of this:" + Environment.NewLine)
                .AppendDescription(reminder.Message);
            channel.SendMessageAsync("", false, message.ToEmbed());
        }

        public async Task StopAsync()
        {
            await this.StopTimers();
            this._isRunning = false;
            Logger.Info("ReminderService stopped.");
        }

        private Task StopTimers()
        {
            foreach (Timer timer in this._currentTimerList.Values)
            {
                timer.Dispose();
            }
            this._currentTimerList.Clear();
            return Task.CompletedTask;
        }

        public async Task AddNew(TRemindEntry reminder)
        {
            await this._reminderDao.AddAsync(reminder);
            await StartAsync();
        }

        public async Task Remove(ulong userId, int userEntryNumber)
        {
            await this._reminderDao.RemoveAsync(userId, userEntryNumber);
            StopTimer(this.BuildTimerKey(userId, userEntryNumber));
        }

        private void StopTimer(string key)
        {
            if (this._currentTimerList.TryGetValue(key, out Timer timer))
            {
                timer.Dispose();
                this._currentTimerList.Remove(key);
            }
        }
    }
}
