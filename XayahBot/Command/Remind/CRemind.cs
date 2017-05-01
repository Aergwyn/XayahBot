using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Database.Model;
using XayahBot.Error;
using XayahBot.Utility;

namespace XayahBot.Command.Remind
{
    [Group("remind me")]
    public class CRemind : ModuleBase
    {
        private readonly string _reminderCreated = "I'm going to remind you in `{0}` {1}{2}. Maybe...";
        private readonly string _createRemindFailed = "Failed to create reminder.";

        //

        private RemindService _remindService { get; set; }

        public CRemind(RemindService remindService)
        {
            this._remindService = remindService;
        }

        //

        [Command("d")]
        [Summary("Reminds you in X days with your specified message.")]
        public async Task Days(int d, [Remainder] string text)
        {
            string message = string.Empty;
            d = this.SetDayInRange(d);
            try
            {
                await this._remindService.AddNew(this.Context.Client as DiscordSocketClient, new TRemindEntry
                {
                    ExpirationDate = DateTime.UtcNow.AddDays(d),
                    Message = text.Trim(),
                    UserId = this.Context.User.Id
                });
                message = string.Format(this._reminderCreated, d, "day", this.AddSForMultiple(d));
            }
            catch (NotSavedException)
            {
                message = this._createRemindFailed;
            }
            await this.ReplyAsync(message);
        }

        [Command("h")]
        [Summary("Reminds you in X hours with your specified message.")]
        public async Task Hours(int h, [Remainder] string text)
        {
            string message = string.Empty;
            h = this.SetOtherInRange(h, 1, 23);
            try
            {
                await this._remindService.AddNew(this.Context.Client as DiscordSocketClient, new TRemindEntry
                {
                    ExpirationDate = DateTime.UtcNow.AddHours(h),
                    Message = text.Trim(),
                    UserId = this.Context.User.Id
                });
                message = string.Format(this._reminderCreated, h, "hour", this.AddSForMultiple(h));
            }
            catch (NotSavedException)
            {
                message = this._createRemindFailed;
            }
            await this.ReplyAsync(message);
        }

        [Command("m")]
        [Summary("Reminds you in X minutes with your specified message.")]
        public async Task Mins(int m, [Remainder] string text)
        {
            string message = string.Empty;
            m = this.SetOtherInRange(m, 5, 59);
            try
            {
                await this._remindService.AddNew(this.Context.Client as DiscordSocketClient, new TRemindEntry
                {
                    ExpirationDate = DateTime.UtcNow.AddMinutes(m),
                    Message = text.Trim(),
                    UserId = this.Context.User.Id
                });
                message = string.Format(this._reminderCreated, m, "minute", this.AddSForMultiple(m));
            }
            catch (NotSavedException)
            {
                message = this._createRemindFailed;
            }
            await this.ReplyAsync(message);
        }

        [Command("not")]
        [Summary("Removes a reminder (by ID!) from your list.")]
        public async Task Not(int id)
        {
            string message = string.Empty;
            try
            {
                await this._remindService.Remove(id, this.Context.User.Id);
                message = $"Removed reminder with ID `{id}` from your list.";
            }
            catch (NotExistingException)
            {
                message = $"Reminder with ID `{id}` does not exist on your list.";
            }
            catch (NotSavedException)
            {
                message = $"Failed to remove reminder with ID `{id}`.";
            }
            await this.ReplyAsync(message);
        }

        private int SetDayInRange(int days)
        {
            int min = 1;
            int max = int.Parse(Property.RemindMaxDays.Value);
            if (days < min)
            {
                days = min;
            }
            else if (days > max)
            {
                days = max;
            }
            return days;
        }

        private int SetOtherInRange(int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }

        private string AddSForMultiple(int value)
        {
            string text = string.Empty;
            if (value > 1)
            {
                text = "s";
            }
            return text;
        }
    }
}
