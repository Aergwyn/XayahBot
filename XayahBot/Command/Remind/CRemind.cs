using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.Logic;
using XayahBot.Database.DAO;
using XayahBot.Database.Model;
using XayahBot.Extension;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Remind
{
    [Group("remind me")]
    public class CRemind : ToggleableModuleBase
    {
        private readonly RemindService _remindService;
        private readonly ReminderDAO _reminderDAO = new ReminderDAO();

        public CRemind(RemindService remindService)
        {
            this._remindService = remindService;
        }

        protected override Property GetDisableProperty()
        {
            return Property.RemindDisabled;
        }

        [Command]
        public Task RemindMe(int value, [OverrideTypeReader(typeof(TimeUnitTypeReader))]TimeUnit timeUnit, [Remainder] string text)
        {
            Task.Run(() => this.CreateReminder(value, timeUnit, text));
            return Task.CompletedTask;
        }

        private async Task CreateReminder(int value, TimeUnit timeUnit, string text)
        {
            if (this.IsDisabled())
            {
                this.NotifyDisabledCommand();
                return;
            }
            text = text.Trim();
            DateTime expirationTime = DateTime.UtcNow;
            int maxTime = 30;
            if (timeUnit == TimeUnit.Day)
            {
                value = this.SetValueInRange(value, 1, maxTime);
                expirationTime = DateTime.UtcNow.AddDays(value);
            }
            else if (timeUnit == TimeUnit.Hour)
            {
                value = this.SetValueInRange(value, 1, maxTime * 24);
                expirationTime = DateTime.UtcNow.AddHours(value);
            }
            else // Minute == Default
            {
                value = this.SetValueInRange(value, 1, maxTime * 24 * 60);
                expirationTime = DateTime.UtcNow.AddMinutes(value);
            }
            int maxLength = 80;
            if (text.Length > maxLength)
            {
                text = text.Substring(0, maxLength);
            }
            await this._remindService.AddNewAsync(new TReminder
            {
                ExpirationTime = expirationTime,
                Message = text,
                UserId = this.Context.User.Id
            });
            FormattedEmbedBuilder message = new FormattedEmbedBuilder()
                .AppendTitle($"{XayahReaction.Success} Done")
                .AppendDescription($"I'm going to remind you at `{expirationTime} UTC`.{Environment.NewLine}I think...");
            if (!(this.Context as CommandContext).IsPrivate)
            {
                message.CreateFooter(this.Context);
            }
            await this.ReplyAsync(message);
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

        [Command("list")]
        public Task List()
        {
            Task.Run(() => this.ListReminder());
            return Task.CompletedTask;
        }

        private async Task ListReminder()
        {
            if (this.IsDisabled())
            {
                this.NotifyDisabledCommand();
                return;
            }
            IMessageChannel channel = await ChannelProvider.GetDMChannelAsync(this.Context);
            List<TReminder> reminders = this._reminderDAO.GetAll(this.Context.User.Id);
            IOrderedEnumerable<TReminder> orderedList = reminders.OrderBy(x => x.ExpirationTime);

            FormattedEmbedBuilder message = new FormattedEmbedBuilder()
                .AppendTitle($"{XayahReaction.Time} Active reminder");
            if (orderedList.Count() > 0)
            {
                foreach (TReminder entry in orderedList)
                {
                    message.AddField($"Expires: {entry.ExpirationTime} UTC", entry.Message, inline: false);
                }
            }
            else
            {
                message.AppendDescription("imagine a soulrending void", AppendOption.Italic);
            }
            await this.ReplyAsync(message);
        }

        [Command("clear")]
        public Task Clear()
        {
            Task.Run(() => this.ClearReminder());
            return Task.CompletedTask;
        }

        private async Task ClearReminder()
        {
            if (this.IsDisabled())
            {
                this.NotifyDisabledCommand();
                return;
            }
            IMessageChannel channel = await ChannelProvider.GetDMChannelAsync(this.Context);
            await this._remindService.ClearUserAsync(this.Context.User.Id);
            FormattedEmbedBuilder message = new FormattedEmbedBuilder()
                .AppendTitle($"{XayahReaction.Success} Done")
                .AppendDescription("I purged all of your reminder for you.");
            await this.ReplyAsync(message);
        }
    }
}