using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Command.System;
using XayahBot.Database.Model;
using XayahBot.Database.Service;

namespace XayahBot.Command.Ignore
{
    [Group("list")]
    public class CList : ModuleBase
    {
        private readonly string _emptyIgnoreList = "This list is empty right now.";

        //

        private readonly IgnoreService _ignoreService = new IgnoreService();

        [Command("ignore")]
        [RequireMod]
        [RequireContext(ContextType.Guild)]
        [Summary("Displays the ignore list.")]
        public async Task Ignore()
        {
            string message = string.Empty;
            List<TIgnoreEntry> ignoreList = this._ignoreService.GetIgnoreList(this.Context.Guild.Id);
            message = $"__Ignored user__{Environment.NewLine}```";
            message += this.BuildListString(ignoreList.Where(x => !x.IsChannel));
            message += $"```{Environment.NewLine}__Ignored channel__{Environment.NewLine}```";
            message += this.BuildListString(ignoreList.Where(x => x.IsChannel));
            message += "```";
            await this.ReplyAsync(message);
        }

        private string BuildListString(IEnumerable<TIgnoreEntry> list)
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
                text += this._emptyIgnoreList;
            }
            return text;
        }
    }
}
