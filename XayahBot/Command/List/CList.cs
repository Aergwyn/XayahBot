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
            List<TIgnoreEntry> ignoreList = this._ignoreDao.GetIgnoreList(this.Context.Guild.Id);
            DiscordFormatMessage message = new DiscordFormatMessage();
            message.Append("Ignored user", AppendOption.UNDERSCORE);
            message.AppendCodeBlock(this.BuildIgnoreListString(ignoreList.Where(x => !x.IsChannel)));
            message.Append("Ignored channel", AppendOption.UNDERSCORE);
            message.AppendCodeBlock(this.BuildIgnoreListString(ignoreList.Where(x => x.IsChannel)));
            await this.ReplyAsync(message.ToString());
        }

        [Command("reminder")]
        [Summary("Displays a list of your reminders.")]
        public async Task Reminder()
        {
            IMessageChannel channel = await this._responseHelper.GetDMChannel(this.Context);
            List<TRemindEntry> reminders = this._remindDao.GetReminders(this.Context.User.Id);
            DiscordFormatMessage message = new DiscordFormatMessage();
            message.Append("Active Reminder", AppendOption.UNDERSCORE);
            message = this.BuildReminderListString(reminders, message);
            await channel.SendMessageAsync(message.ToString());
        }

        private string BuildIgnoreListString(IEnumerable<TIgnoreEntry> list)
        {
            string text = string.Empty;
            IOrderedEnumerable<TIgnoreEntry> orderedList = list.OrderBy(x => x.SubjectName);
            if (orderedList.Count() > 0)
            {
                for(int i = 0; i < orderedList.Count(); i++)
                {
                    if (i > 0)
                    {
                        text += Environment.NewLine;
                    }
                    text += orderedList.ElementAt(i).SubjectName;
                }
            }
            else
            {
                text += this._emptyList;
            }
            return text;
        }

        private DiscordFormatMessage BuildReminderListString(IEnumerable<TRemindEntry> list, DiscordFormatMessage message)
        {
            IOrderedEnumerable<TRemindEntry> orderedList = list.OrderBy(x => x.Id);
            if (orderedList.Count() > 0)
            {
                foreach (TRemindEntry entry in orderedList)
                {
                    message.AppendCodeBlock($"ID: {entry.Id} | Expires: {entry.ExpirationDate} UTC{Environment.NewLine}" +
                        $"Message:{Environment.NewLine}{entry.Message}");
                }
            }
            else
            {
                message.AppendCodeBlock(this._emptyList);
            }
            return message;
        }
    }
}
