using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Command.Attribute;
using XayahBot.Database.Model;
using XayahBot.Database.Service;

namespace XayahBot.Command
{
    [Group("list")]
    public class CList : ModuleBase
    {
        private readonly string _emptyIgnoreList = "This list is empty right now.";

        //

        private readonly IgnoreService _ignoreService;

        public CList(IgnoreService ignoreService)
        {
            this._ignoreService = ignoreService;
        }

        //

        [Command("ignore")]
        [RequireMod]
        [RequireContext(ContextType.Guild)]
        [Summary("Displays the ignore list.")]
        public Task Ignore()
        {
            string message = string.Empty;
            List<TIgnoreEntry> ignoreList = this._ignoreService.GetIgnoreList(this.Context.Guild.Id);
            message = $"__Ignored user__{Environment.NewLine}```";
            message += this.ListIgnore(ignoreList.Where(x => !x.IsChannel).ToList());
            message += $"```{Environment.NewLine}__Ignored channel__{Environment.NewLine}```";
            message += this.ListIgnore(ignoreList.Where(x => x.IsChannel).ToList());
            message += "```";
            ReplyAsync(message);
            return Task.CompletedTask;
        }

        private string ListIgnore(List<TIgnoreEntry> list)
        {
            string text = string.Empty;
            IOrderedEnumerable<TIgnoreEntry> orderedList = list.OrderBy(x => x.SubjectName);
            if (orderedList.Count() > 0)
            {
                for (int i = 0; i < orderedList.Count(); i++)
                {
                    text += Environment.NewLine + orderedList.ElementAt(i).SubjectName;
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
