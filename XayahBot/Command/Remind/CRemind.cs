#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Database.DAO;
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
        private readonly RemindDAO _remindDao = new RemindDAO();

        public CRemind(RemindService remindService)
        {
            this._remindService = remindService;
        }

        //

        [Command("list")]
        [Summary("Displays a list of your active reminder.")]
        public async Task Reminder()
        {
            IMessageChannel channel = await ResponseHelper.GetDMChannel(this.Context);
            List<TRemindEntry> reminders = this._remindDao.GetReminders(this.Context.User.Id);
            DiscordFormatMessage message = new DiscordFormatMessage();
            message = this.BuildReminderListString(reminders, message);
            channel.SendMessageAsync(message.ToString());
        }

        private DiscordFormatMessage BuildReminderListString(IEnumerable<TRemindEntry> list, DiscordFormatMessage message)
        {
            IOrderedEnumerable<TRemindEntry> orderedList = list.OrderBy(x => x.Id);
            if (orderedList.Count() > 0)
            {
                message.Append("Active Reminder", AppendOption.UNDERSCORE);
                foreach (TRemindEntry entry in orderedList)
                {
                    message.AppendCodeBlock($"ID: {entry.Id} | Expires: {entry.ExpirationDate} UTC{Environment.NewLine}" +
                        $"Message:{Environment.NewLine}{entry.Message}");
                }
            }
            else
            {
                message.Append("You have no active reminder right now.");
            }
            return message;
        }

        [Command("d")]
        [Summary("Reminds you in 1-30 days with your specified message.")]
        public async Task Days(int d, [Remainder] string text)
        {
            text = text.Trim();
            string message = string.Empty;
            d = this.SetValueInRange(d, 1, 30);
            try
            {
                await this._remindService.AddNew(this.Context.Client as DiscordSocketClient, new TRemindEntry
                {
                    ExpirationDate = DateTime.UtcNow.AddDays(d),
                    Message = text,
                    UserId = this.Context.User.Id
                });
                message = string.Format(this._reminderCreated, d, "day", this.AddSForMultiple(d));
            }
            catch (NotSavedException)
            {
                message = this._createRemindFailed;
            }
            this.ReplyAsync(message);
        }

        [Command("h")]
        [Summary("Reminds you in 1-23 hours with your specified message.")]
        public async Task Hours(int h, [Remainder] string text)
        {
            text = text.Trim();
            string message = string.Empty;
            h = this.SetValueInRange(h, 1, 23);
            try
            {
                await this._remindService.AddNew(this.Context.Client as DiscordSocketClient, new TRemindEntry
                {
                    ExpirationDate = DateTime.UtcNow.AddHours(h),
                    Message = text,
                    UserId = this.Context.User.Id
                });
                message = string.Format(this._reminderCreated, h, "hour", this.AddSForMultiple(h));
            }
            catch (NotSavedException)
            {
                message = this._createRemindFailed;
            }
            this.ReplyAsync(message);
        }

        [Command("m")]
        [Summary("Reminds you in 15-59 minutes with your specified message.")]
        public async Task Mins(int m, [Remainder] string text)
        {
            text = text.Trim();
            string message = string.Empty;
            m = this.SetValueInRange(m, 15, 59);
            try
            {
                await this._remindService.AddNew(this.Context.Client as DiscordSocketClient, new TRemindEntry
                {
                    ExpirationDate = DateTime.UtcNow.AddMinutes(m),
                    Message = text,
                    UserId = this.Context.User.Id
                });
                message = string.Format(this._reminderCreated, m, "minute", this.AddSForMultiple(m));
            }
            catch (NotSavedException)
            {
                message = this._createRemindFailed;
            }
            this.ReplyAsync(message);
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
            this.ReplyAsync(message);
        }

        private int SetValueInRange(int value, int min, int max)
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
