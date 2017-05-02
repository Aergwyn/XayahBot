#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.System;
using XayahBot.Database.Service;
using XayahBot.Error;
using XayahBot.Utility;

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
            await this.SendReplies();
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
                Logger.Warning(nsex.Message, nsex);
            }
        }

        private async Task SendReplies()
        {
            string message = this.CreateMessage();
            if (!string.IsNullOrWhiteSpace(message))
            {
                await this.ReplyAsync(message);
            }
            if (this._hasNewUnignoredUser)
            {
                await this.ReplyAsync(RNG.FromList(this._unignoredReactionList));
            }
        }

        private string CreateMessage()
        {
            string text = string.Empty;
            if (this._newUnignoredList.Count > 0)
            {
                text += $"Removed {this.BuildEnumerationFromList(this._newUnignoredList)} from the ignore list.";
            }
            int notExistingCount = this._notExistingUnignoredList.Count;
            if (notExistingCount > 0)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    text += Environment.NewLine + Environment.NewLine;
                }
                text += $"{this.BuildEnumerationFromList(this._notExistingUnignoredList)} " +
                    $"{this.GetSingularOrPlural(notExistingCount)} never on the ignore list.";
            }
            return text;
        }

        private string BuildEnumerationFromList(List<string> list)
        {
            string text = string.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0)
                {
                    if (i == list.Count() - 1)
                    {
                        text += " and ";
                    }
                    else
                    {
                        text += ", ";
                    }
                }
                text += $"`{list.ElementAt(i)}`";
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
