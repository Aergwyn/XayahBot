#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.System;
using XayahBot.Database.DAO;
using XayahBot.Error;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Ignore
{
    public class CUnignore : LoggedModuleBase
    {
        private readonly List<string> _unignoredReactionList = new List<string>
        {
            "Maybe I over did it.",
            "Well, that's just an invasion of privacy!",
            "Don't embarass me!",
            "But is it really okay?"
        };

        //

        private readonly IgnoreDAO _ignoreDao = new IgnoreDAO();
        private bool _hasNewUnignoredUser = false;
        private List<string> _newUnignoredList = new List<string>();
        private List<string> _notExistingUnignoredList = new List<string>();

        [Command("unignore")]
        [RequireMod]
        [RequireContext(ContextType.Guild)]
        [Summary("Removes all mentioned user and channel from the ignore list.")]
        public async Task Unignore([Remainder] string text)
        {
            string message = string.Empty;
            foreach (ulong userId in this.Context.Message.MentionedUserIds.Distinct())
            {
                IUser user = await this.Context.Guild.GetUserAsync(userId);
                if (this.IsActualUser(user))
                {
                    await RemoveIgnore(user.Id, user.ToString());
                }
            }
            foreach (ulong channelId in this.Context.Message.MentionedChannelIds.Distinct())
            {
                IChannel channel = await this.Context.Guild.GetChannelAsync(channelId);
                await RemoveIgnore(channel.Id, channel.Name, true);
            }
            this.SendReplies();
        }

        private bool IsActualUser(IUser user)
        {
            if (!user.Id.Equals(this.Context.Client.CurrentUser.Id) && !DiscordPermissions.IsOwner(user))
            {
                return true;
            }
            return false;
        }

        private async Task RemoveIgnore(ulong subjectId, string subjectName)
        {
            await this.RemoveIgnore(subjectId, subjectName, false);
        }

        private async Task RemoveIgnore(ulong subjectId, string subjectName, bool isChannel)
        {
            try
            {
                await this._ignoreDao.RemoveAsync(this.Context.Guild.Id, subjectId);
                this._newUnignoredList.Add(subjectName);
                if (!isChannel)
                {
                    this._hasNewUnignoredUser = true;
                }
            }
            catch (NotExistingException)
            {
                this._notExistingUnignoredList.Add(subjectName);
            }
            catch (NotSavedException nsex)
            {
                Logger.Error($"Failed to remove {subjectId} from ignore for {this.Context.User.ToString()}.", nsex);
            }
        }

        private async Task SendReplies()
        {
            string text = this.CreateMessage();
            if (!string.IsNullOrWhiteSpace(text))
            {
                DiscordFormatEmbed message = new DiscordFormatEmbed(this.Context)
                .AppendTitle(":no_entry_sign: ")
                .AppendTitle("Ignore List", AppendOption.Underscore)
                .AppendDescription(this.CreateMessage());
                await this.ReplyAsync("", false, message.ToEmbed());
            }
            if (this._hasNewUnignoredUser)
            {
                this.ReplyAsync(RNG.FromList(this._unignoredReactionList));
            }
        }

        private string CreateMessage()
        {
            string text = string.Empty;
            if (this._newUnignoredList.Count > 0)
            {
                text += $"Removed {ListUtil.BuildAndEnumeration(this._newUnignoredList)} from the ignore list.";
            }
            int notExistingCount = this._notExistingUnignoredList.Count;
            if (notExistingCount > 0)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    text += Environment.NewLine;
                }
                text += $"{ListUtil.BuildAndEnumeration(this._notExistingUnignoredList)} " +
                    $"{this.GetSingularOrPlural(notExistingCount)} never on the ignore list.";
            }
            return text;
        }

        private string GetSingularOrPlural(int count)
        {
            string text = string.Empty;
            if (count > 1)
            {
                text = "were";
            }
            else
            {
                text = "was";
            }
            return text;
        }
    }
}
