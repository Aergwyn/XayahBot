using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.System;
using XayahBot.Database.Model;
using XayahBot.Database.Service;
using XayahBot.Utility;

namespace XayahBot.Command.List
{
    [Group("list")]
    public class CList : ModuleBase
    {
        private readonly string _emptyList = "This list is empty right now.";

        //

        private readonly IgnoreDAO _ignoreDao = new IgnoreDAO();
        private readonly RemindDAO _remindDao = new RemindDAO();
        private readonly ResponseHelper _responseHelper = new ResponseHelper();

        [Command("ignore")]
        [RequireMod]
        [RequireContext(ContextType.Guild)]
        [Summary("Displays the ignore list.")]
        public async Task Ignore()
        {
            string message = string.Empty;
            List<TIgnoreEntry> ignoreList = this._ignoreDao.GetIgnoreList(this.Context.Guild.Id);
            message = $"__Ignored user__{Environment.NewLine}```";
            message += this.BuildIgnoreListString(ignoreList.Where(x => !x.IsChannel));
            message += $"```{Environment.NewLine}__Ignored channel__{Environment.NewLine}```";
            message += this.BuildIgnoreListString(ignoreList.Where(x => x.IsChannel));
            message += "```";
            await this.ReplyAsync(message);
        }

        [Command("reminder")]
        [Summary("Displays a list of your reminders.")]
        public async Task Reminder()
        {
            string message = string.Empty;
            IMessageChannel channel = await this._responseHelper.GetDMChannel(this.Context);
            List<TRemindEntry> reminders = this._remindDao.GetReminders(this.Context.User.Id);
            message = $"__Active Reminder__{Environment.NewLine}";
            message += this.BuildReminderListString(reminders);
            await channel.SendMessageAsync(message);
        }

        private string BuildIgnoreListString(IEnumerable<TIgnoreEntry> list)
        {
            string text = string.Empty;
            IOrderedEnumerable<TIgnoreEntry> orderedList = list.OrderBy(x => x.SubjectName);
            if (orderedList.Count() > 0)
            {
                foreach(TIgnoreEntry entry in orderedList)
                {
                    text += Environment.NewLine + entry.SubjectName;
                }
            }
            else
            {
                text += this._emptyList;
            }
            return text;
        }

        private string BuildReminderListString(IEnumerable<TRemindEntry> list)
        {
            string text = string.Empty;
            IOrderedEnumerable<TRemindEntry> orderedList = list.OrderBy(x => x.Id);
            if (orderedList.Count() > 0)
            {
                foreach (TRemindEntry entry in orderedList)
                {
                    text += $"```{Environment.NewLine}ID: {entry.Id} | Expires: {entry.ExpirationDate}{Environment.NewLine}Message: {entry.Message}```";
                }
            }
            else
            {
                text += $"```{Environment.NewLine}{this._emptyList}```";
            }
            return text;
        }
    }
}
