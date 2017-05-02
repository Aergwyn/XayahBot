#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.System;
using XayahBot.Database.Model;
using XayahBot.Database.Service;
using XayahBot.Error;
using XayahBot.Utility;

namespace XayahBot.Command.Ignore
{
    public class CIgnore : LoggedModuleBase
    {
        private readonly List<string> _ignoredReactionList = new List<string>
        {
            "I warned you. Oh wait... did I? Crap. My fault.",
            "All out of second chances, sorry!",
            "And they won't be needing that anymore.",
            "My last nerve is long gone."
        };

        //

        private readonly IgnoreDAO _ignoreDao = new IgnoreDAO();
        private bool _hasNewIgnoredUser = false;
        private List<string> _newIgnoredList = new List<string>();
        private List<string> _existingIgnoredList = new List<string>();

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

        private string BuildIgnoreListString(IEnumerable<TIgnoreEntry> list)
        {
            string text = string.Empty;
            IOrderedEnumerable<TIgnoreEntry> orderedList = list.OrderBy(x => x.SubjectName);
            if (orderedList.Count() > 0)
            {
                for (int i = 0; i < orderedList.Count(); i++)
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
                text += "This list is empty right now.";
            }
            return text;
        }

        [Command("ignore")]
        [RequireMod]
        [RequireContext(ContextType.Guild)]
        [Summary("Adds all mentioned user and channel to the ignore list.")]
        public async Task Ignore([Remainder] string text)
        {
            foreach (ulong userId in this.Context.Message.MentionedUserIds.Distinct())
            {
                IUser user = await this.Context.Guild.GetUserAsync(userId);
                if (this.IsActualUser(user))
                {
                    await this.AddToIgnore(user.Id, user.ToString());
                }
            }
            foreach (ulong channelId in this.Context.Message.MentionedChannelIds.Distinct())
            {
                IChannel channel = await this.Context.Guild.GetChannelAsync(channelId);
                await this.AddToIgnore(channel.Id, channel.Name, true);
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

        private async Task AddToIgnore(ulong subjectId, string subjectName)
        {
            await this.AddToIgnore(subjectId, subjectName, false);
        }

        private async Task AddToIgnore(ulong subjectId, string subjectName, bool isChannel)
        {
            try
            {
                await this._ignoreDao.AddAsync(new TIgnoreEntry
                {
                    GuildId = this.Context.Guild.Id,
                    IsChannel = isChannel,
                    SubjectId = subjectId,
                    SubjectName = subjectName
                });
                this._newIgnoredList.Add(subjectName);
                if (!isChannel)
                {
                    this._hasNewIgnoredUser = true;
                }
            }
            catch (AlreadyExistingException)
            {
                this._existingIgnoredList.Add(subjectName);
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
            if (this._hasNewIgnoredUser)
            {
                await this.ReplyAsync(RNG.FromList(this._ignoredReactionList));
            }
        }

        private string CreateMessage()
        {
            string text = string.Empty;
            if (this._newIgnoredList.Count > 0)
            {
                text += $"Added {this.BuildEnumerationFromList(this._newIgnoredList)} to the ignore list.";
            }
            int existingCount = this._existingIgnoredList.Count;
            if (existingCount > 0)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    text += Environment.NewLine + Environment.NewLine;
                }
                text += $"{this.BuildEnumerationFromList(this._existingIgnoredList)} " +
                    $"{this.GetSingularOrPlural(existingCount)} already on the ignore list.";
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
                text = "are";
            }
            else
            {
                text = "is";
            }
            return text;
        }
    }
}
