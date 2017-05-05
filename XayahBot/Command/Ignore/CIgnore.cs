#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.System;
using XayahBot.Database.DAO;
using XayahBot.Database.Model;
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
        public Task Ignore()
        {
            List<TIgnoreEntry> ignoreList = this._ignoreDao.GetIgnoreList(this.Context.Guild.Id);

            DiscordFormatEmbed message = new DiscordFormatEmbed(this.Context)
                .AppendTitle(":no_entry_sign: ")
                .AppendTitle("Ignore List", AppendOption.Underscore)
                .AppendDescription("Here is the current ignore list for this server." + Environment.NewLine + Environment.NewLine)
                .AppendDescription("Ignored User", AppendOption.Bold)
                .AppendDescriptionCodeBlock(this.BuildIgnoreListString(ignoreList.Where(x => !x.IsChannel)))
                .AppendDescription("Ignored Channel", AppendOption.Bold)
                .AppendDescriptionCodeBlock(this.BuildIgnoreListString(ignoreList.Where(x => x.IsChannel)));
            this.ReplyAsync("", false, message.ToEmbed());
            return Task.CompletedTask;
        }

        private string BuildIgnoreListString(IEnumerable<TIgnoreEntry> list)
        {
            string text = string.Empty;
            IOrderedEnumerable<TIgnoreEntry> orderedList = list.OrderBy(x => x.SubjectName);
            if (orderedList.Count() > 0)
            {
                text = ListUtil.BuildEnumeration(list);
            }
            else
            {
                text = "This list is empty right now.";
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
                Logger.Error($"Failed to add {subjectId} to ignore for {this.Context.User.ToString()}.", nsex);
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
                    .AppendDescription(text);
                await this.ReplyAsync("", false, message.ToEmbed());
            }
            if (this._hasNewIgnoredUser)
            {
                this.ReplyAsync(RNG.FromList(this._ignoredReactionList));
            }
        }

        private string CreateMessage()
        {
            string text = string.Empty;
            if (this._newIgnoredList.Count > 0)
            {
                text += $"Added {ListUtil.BuildAndEnumeration(this._newIgnoredList)} to the ignore list.";
            }
            int existingCount = this._existingIgnoredList.Count;
            if (existingCount > 0)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    text += Environment.NewLine;
                }
                text += $"{ListUtil.BuildAndEnumeration(this._existingIgnoredList)} " +
                    $"{this.GetSingularOrPlural(existingCount)} already on the ignore list.";
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
