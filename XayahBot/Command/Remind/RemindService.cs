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
        private readonly ResponseHelper _responseHelper = new ResponseHelper();
        private bool _isRunning = false;
        private Dictionary<int, Timer> _currentTimer = new Dictionary<int, Timer>();

        public async Task Start()
        {
            await this._lock.WaitAsync();
            try
            {
                if (!this._isRunning)
                {
                    this._isRunning = true;
                    Task.Run(() => Run());
                }
            }
            finally
            {
                this._lock.Release();
            }
        }

        public async Task StopAsync()
        {
            this._isRunning = false;
            await StopTimers();
        }

        private async Task Run()
        {
            try
            {
                bool processed = false;
                while (this._isRunning)
                {
                    int interval = int.Parse(Property.RemindInterval.Value);
                    if (!processed && DateTime.UtcNow.Minute % interval == 0)
                    {
                        processed = true;
                        List<TRemindEntry> reminders = this._remindDao.GetReminders();
                        await ProcessExpiringReminders(reminders.Where(x => !this._currentTimer.Keys.Contains(x.Id)), interval);
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
                    this._currentTimer.Add(reminder.Id, new Timer(this.HandleExpiredReminder, reminder, new TimeSpan(remainingTicks), new TimeSpan(Timeout.Infinite)));
                }
            }
            return Task.CompletedTask;
        }

        private async void HandleExpiredReminder(object state)
        {
            TRemindEntry reminder = (TRemindEntry)state;
            StopTimer(reminder.Id);
            IMessageChannel channel = await this._responseHelper.GetDMChannel(this._client, reminder.UserId);
            await channel.SendMessageAsync(reminder.Message);
        }

        private Task StopTimers()
        {
            foreach (Timer timer in this._currentTimer.Values)
            {
                timer.Dispose();
            }
            this._currentTimer.Clear();
            return Task.CompletedTask;
        }

        private void StopTimer(int id)
        {
            if (this._currentTimer.TryGetValue(id, out Timer timer))
            {
                timer.Dispose();
                this._currentTimer.Remove(id);
            }
        }

        //

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
    }
}
