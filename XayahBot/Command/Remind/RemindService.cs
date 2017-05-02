#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using XayahBot.Database.Model;
using XayahBot.Database.Service;
using XayahBot.Utility;

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

        private RemindService(DiscordSocketClient client)
        {
            this._client = client;
        }

        //

        private readonly DiscordSocketClient _client;
        private readonly RemindDAO _remindDao = new RemindDAO();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private bool _isRunning = false;
        private Dictionary<int, Timer> _currentTimerList = new Dictionary<int, Timer>();

        public async Task Start()
        {
            await this._lock.WaitAsync();
            try
            {
                if (!this._isRunning)
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
                    if ((!processed && DateTime.UtcNow.Minute % interval == 0) || init)
                    {
                        init = false;
                        processed = true;
                        List<TRemindEntry> reminders = this._remindDao.GetReminders();
                        await ProcessExpiringReminders(reminders.Where(x => !this._currentTimerList.Keys.Contains(x.Id)), interval);
                    }
                    else
                    {
                        processed = false;
                    }
                    await Task.Delay(15000);
                }
            }
            finally
            {
                await StopAsync();
            }
        }

        private Task ProcessExpiringReminders(IEnumerable<TRemindEntry> list, int interval)
        {
            foreach (TRemindEntry reminder in list)
            {
                if (DateTime.UtcNow.AddMinutes(interval) > reminder.ExpirationDate)
                {
                    long remainingTicks = reminder.ExpirationDate.Ticks - DateTime.UtcNow.Ticks;
                    if (remainingTicks < 0)
                    {
                        remainingTicks = 0;
                    }
                    this._currentTimerList.Add(reminder.Id, new Timer(this.HandleExpiredReminder, reminder, new TimeSpan(remainingTicks), new TimeSpan(Timeout.Infinite)));
                }
            }
            return Task.CompletedTask;
        }

        private async void HandleExpiredReminder(object state)
        {
            TRemindEntry reminder = (TRemindEntry)state;
            await this._remindDao.RemoveAsync(reminder.Id, reminder.UserId);
            StopTimer(reminder.Id);
            IMessageChannel channel = await ResponseHelper.GetDMChannel(this._client, reminder.UserId);
            DiscordFormatMessage message = new DiscordFormatMessage("Back then you told me to remind you of this.");
            message.AppendCodeBlock(reminder.Message);
            await channel.SendMessageAsync(message.ToString());
        }

        public async Task StopAsync()
        {
            this._isRunning = false;
            await StopTimers();
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

        public async Task AddNew(DiscordSocketClient client, TRemindEntry reminder)
        {
            await this._remindDao.AddAsync(reminder);
            await Start();
        }

        public async Task Remove(int id, ulong userId)
        {
            await this._remindDao.RemoveAsync(id, userId);
            StopTimer(id);
        }

        private void StopTimer(int id)
        {
            if (this._currentTimerList.TryGetValue(id, out Timer timer))
            {
                timer.Dispose();
                this._currentTimerList.Remove(id);
            }
        }
    }
}
