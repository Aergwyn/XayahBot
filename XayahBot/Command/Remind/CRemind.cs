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
        private readonly string _reminderCreated = "I'm going to remind you in {0} {1}{2}. Maybe...";
        private readonly string _createRemindFailed = "Failed to create reminder.";
        private readonly string _removedReminder = "Removed reminder with ID {0} from your list.";
        private readonly string _remindNotExisting = "Reminder with ID {0} does not exist on your list.";
        private readonly string _removeRemindFailed = "Failed to remove reminder with ID {0}.";

        //

        private RemindService _remindService { get; set; }

        public CRemind(RemindService remindService)
        {
            this._remindService = remindService;
        }

        //

        [Command("days"), Alias("d")]
        [Summary("Adds a reminder with specified message to your list.")]
        public async Task Days(int days, [Remainder] string text)
        {
            string message = string.Empty;
            days = this.SetDayInRange(days);
            try
            {
                await this._remindService.AddNew(this.Context.Client as DiscordSocketClient, new TRemindEntry
                {
                    ExpirationDate = DateTime.UtcNow.AddDays(days),
                    Message = text.Trim(),
                    UserId = this.Context.User.Id
                });
                message = string.Format(this._reminderCreated, days, "day", this.AddSForMultiple(days));
            }
            catch (NotSavedException)
            {
                message = this._createRemindFailed;
            }
            await this.ReplyAsync(message);
        }

        [Command("hours"), Alias("h")]
        [Summary("Adds a reminder with specified message to your list.")]
        public async Task Hours(int hours, [Remainder] string text)
        {
            string message = string.Empty;
            hours = this.SetHourInRange(hours);
            try
            {
                await this._remindService.AddNew(this.Context.Client as DiscordSocketClient, new TRemindEntry
                {
                    ExpirationDate = DateTime.UtcNow.AddHours(hours),
                    Message = text.Trim(),
                    UserId = this.Context.User.Id
                });
                message = string.Format(this._reminderCreated, hours, "hour", this.AddSForMultiple(hours));
            }
            catch (NotSavedException)
            {
                message = this._createRemindFailed;
            }
            await this.ReplyAsync(message);
        }

        [Command("not")]
        [RequireOwner]
        [Summary("Removes a specific reminder from your list.")]
        public async Task Not(int id)
        {
            string message = string.Empty;
            try
            {
                await this._remindService.Remove(id, this.Context.User.Id);
                message = string.Format(this._removedReminder, id);
            }
            catch (NotExistingException)
            {
                message = string.Format(this._remindNotExisting, id);
            }
            catch (NotSavedException)
            {
                message = string.Format(this._removeRemindFailed, id);
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

        private int SetHourInRange(int hours)
        {
            int min = 1;
            int max = int.Parse(Property.RemindMaxHours.Value);
            if (hours < min)
            {
                hours = min;
            }
            else if (hours > max)
            {
                hours = max;
            }
            return hours;
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
