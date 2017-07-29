using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Command.Precondition;
using XayahBot.Database.DAO;
using XayahBot.Database.Error;
using XayahBot.Database.Model;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Ignore
{
    [Category(CategoryType.IGNORE)]
    public class CIgnore : LoggedModuleBase
    {
        private readonly List<string> _ignoredReactionList = new List<string>
        {
            "I warned you. Oh wait... did I? Crap. My fault.",
            "All out of second chances, sorry!",
            "And they won't be needing that anymore.",
            "My last nerve is long gone."
        };

        private readonly DiscordSocketClient _client;
        private readonly IgnoreListDAO _ignoreListDao = new IgnoreListDAO();
        private bool _hasNewIgnoredUser = false;
        private List<string> _newIgnoredList = new List<string>();
        private List<string> _existingIgnoredList = new List<string>();

        public CIgnore(DiscordSocketClient client)
        {
            this._client = client;
        }

        [Command("ignore")]
        [RequireOwner]
        [RequireContext(ContextType.Guild)]
        [Summary("Displays the ignore list.")]
        public async Task Ignore()
        {
            List<TIgnoreEntry> ignoreList = this._ignoreListDao.GetAll(this.Context.Guild.Id);
            string userString = await this.BuildIgnoreListString(ignoreList.Where(x => !x.IsChannel));
            string channelString = await this.BuildIgnoreListString(ignoreList.Where(x => x.IsChannel));
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .CreateFooter(this.Context)
                .AppendDescription($"Here is the current ignore list for this server.")
                .AppendDescription(Environment.NewLine + Environment.NewLine)
                .AppendDescription("Ignored User", AppendOption.Bold)
                .AppendDescription(Environment.NewLine)
                .AppendDescription(userString)
                .AppendDescription(Environment.NewLine + Environment.NewLine)
                .AppendDescription("Ignored Channel", AppendOption.Bold)
                .AppendDescription(Environment.NewLine)
                .AppendDescription(channelString);
            this.ReplyAsync("", false, message.ToEmbed());
        }

        private async Task<string> BuildIgnoreListString(IEnumerable<TIgnoreEntry> ignoreList)
        {
            string text = string.Empty;
            List<string> names = new List<string>();
            foreach (TIgnoreEntry ignoreEntry in ignoreList)
            {
                if (ignoreEntry.IsChannel)
                {
                    IChannel channel = await this.Context.Guild.GetChannelAsync(ignoreEntry.SubjectId) as IChannel;
                    names.Add(channel.Name);
                }
                else
                {
                    IUser user = this._client.GetUser(ignoreEntry.SubjectId) as IUser;
                    names.Add(user.ToString());
                }
            }
            text = ListUtil.BuildEnumeration(names);
            if (string.IsNullOrWhiteSpace(text))
            {
                text = "This list is empty right now.";
            }
            return text;
        }

        [Command("ignore")]
        [RequireOwner]
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
            this.SendResponse();
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
                TIgnoreEntry entry = new TIgnoreEntry
                {
                    GuildId = this.Context.Guild.Id,
                    IsChannel = isChannel,
                    SubjectId = subjectId
                };
                await this._ignoreListDao.SaveAsync(entry);
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

        private async Task SendResponse()
        {
            string text = this.CreateMessage();
            if (!string.IsNullOrWhiteSpace(text))
            {
                DiscordFormatEmbed message = new DiscordFormatEmbed()
                    .CreateFooter(this.Context)
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
