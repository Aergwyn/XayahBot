#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.Precondition;
using XayahBot.Database.DAO;
using XayahBot.Database.Error;
using XayahBot.Database.Model;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Remind
{
    [Group("remind me")]
    [Category(CategoryType.REMIND)]
    public class CRemind : ModuleBase
    {
        private readonly RemindService _remindService;
        private readonly ReminderDAO _reminderDao = new ReminderDAO();

        public CRemind(RemindService remindService)
        {
            this._remindService = remindService;
        }

        [Command]
        [Summary("Displays a list of your active reminder.")]
        public async Task List()
        {
            IMessageChannel channel = await ChannelRetriever.GetDMChannel(this.Context);
            List<TRemindEntry> reminders = this._reminderDao.GetAll(this.Context.User.Id);
            channel.SendMessageAsync("", false, this.BuildReminderResponse(reminders).ToEmbed());
        }

        private DiscordFormatEmbed BuildReminderResponse(IEnumerable<TRemindEntry> list)
        {
            DiscordFormatEmbed message = new DiscordFormatEmbed();
            IOrderedEnumerable<TRemindEntry> orderedList = list.OrderBy(x => x.ExpirationTime);
            if (orderedList.Count() > 0)
            {
                message.AppendDescription("Here is a list of your active reminder.");
                foreach (TRemindEntry entry in orderedList)
                {
                    message.AddField($"ID: {entry.UserEntryNumber} | Expires: {entry.ExpirationTime} UTC", entry.Message);
                }
            }
            else
            {
                message.AppendDescription("You have no active reminder right now.");
            }
            return message;
        }

        [Command("d")]
        [Summary("Reminds you in 1-30 days with your specified message.")]
        public async Task Days(int d, [Remainder] string text)
        {
            text = text.Trim();
            d = this.SetValueInRange(d, 1, 30);
            if (this.IsValidReminder(text) && await CreateReminder(text, DateTime.UtcNow.AddDays(d)))
            {
                this.SendSuccessResponse("day", d);
            }
        }

        [Command("h")]
        [Summary("Reminds you in 1-23 hours with your specified message.")]
        public async Task Hours(int h, [Remainder] string text)
        {
            text = text.Trim();
            h = this.SetValueInRange(h, 1, 23);
            if (this.IsValidReminder(text) && await CreateReminder(text, DateTime.UtcNow.AddHours(h)))
            {
                this.SendSuccessResponse("hour", h);
            }
        }

        [Command("m")]
        [Summary("Reminds you in 15-59 minutes with your specified message.")]
        public async Task Mins(int m, [Remainder] string text)
        {
            text = text.Trim();
            m = this.SetValueInRange(m, 15, 59);
            if (this.IsValidReminder(text) && await CreateReminder(text, DateTime.UtcNow.AddMinutes(m)))
            {
                this.SendSuccessResponse("minute", m);
            }
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

        private bool IsValidReminder(string textToCheck)
        {
            int reminderCap = int.Parse(Property.ReminderCap.Value);
            int reminderTextCap = int.Parse(Property.ReminderTextCap.Value);
            if (this._reminderDao.GetAll(this.Context.User.Id).Count >= reminderCap)
            {
                this.SendResponse($"You already reached the maximum of {reminderCap} active reminder.");
                return false;
            }
            if (textToCheck.Length > reminderTextCap)
            {
                this.SendResponse($"Your message contains more than {reminderTextCap} characters and is too long.");
                return false;
            }
            return true;
        }

        private void SendResponse(string text)
        {
            DiscordFormatEmbed message = null;
            message = new DiscordFormatEmbed();
            if (!this.Context.IsPrivate)
            {
                message.CreateFooter(this.Context);
            }
            message.AppendDescription(text);
            this.ReplyAsync("", false, message.ToEmbed());
        }

        private async Task<bool> CreateReminder(string text, DateTime expirationTime)
        {
            try
            {
                await this._remindService.AddNewAsync(new TRemindEntry
                {
                    ExpirationTime = expirationTime,
                    Message = text,
                    UserId = this.Context.User.Id
                });
                return true;
            }
            catch (NotSavedException nsex)
            {
                Logger.Error($"Failed to create reminder for {this.Context.User.ToString()}.", nsex);
            }
            return false;
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

        private void SendSuccessResponse(string timeUnit, int timeValue)
        {
            string text = string.Format("I'm going to remind you in `{0}` {1}{2}. I think...", timeValue, timeUnit, this.AddSForMultiple(timeValue));
            this.SendResponse(text);
        }

        [Command("not")]
        [Summary("Removes a reminder (by ID!) from your list.")]
        public async Task Not(int id)
        {
            IMessageChannel channel = await ChannelRetriever.GetDMChannel(this.Context);
            DiscordFormatEmbed message = new DiscordFormatEmbed();
            try
            {
                await this._remindService.RemoveAsync(this.Context.User.Id, id);
                message.AppendDescription($"Removed reminder with ID `{id}` from your list.");
            }
            catch (NotExistingException)
            {
                message.AppendDescription($"Reminder with ID `{id}` does not exist on your list.");
            }
            catch (NotSavedException nsex)
            {
                Logger.Error($"Failed to remove reminder with ID {id} for {this.Context.User}.", nsex);
            }
            channel.SendMessageAsync("", false, message.ToEmbed());
        }
    }
}