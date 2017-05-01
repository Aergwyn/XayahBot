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

        private readonly RNG _random = new RNG();
        private readonly Permission _permission = new Permission();
        private readonly IgnoreDAO _ignoreDao = new IgnoreDAO();

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
            string message = string.Empty;
            foreach (ulong userId in this.Context.Message.MentionedUserIds.Distinct())
            {
                IUser user = await this.Context.Guild.GetUserAsync(userId);
                if (this.IsActualUser(user))
                {
                    message += await this.AddToIgnore(user.Id, user.ToString()) + Environment.NewLine;
                }
            }
            foreach (ulong channelId in this.Context.Message.MentionedChannelIds.Distinct())
            {
                IChannel channel = await this.Context.Guild.GetChannelAsync(channelId);
                message += await this.AddToIgnore(channel.Id, channel.Name, true) + Environment.NewLine;
            }
            await this.SendReplies(message);
        }

        private bool IsActualUser(IUser user)
        {
            if (!user.Id.Equals(this.Context.Client.CurrentUser.Id) && !this._permission.IsOwner(user))
            {
                return true;
            }
            return false;
        }

        private async Task<string> AddToIgnore(ulong subjectId, string subjectName)
        {
            return await this.AddToIgnore(subjectId, subjectName, false);
        }

        private async Task<string> AddToIgnore(ulong subjectId, string subjectName, bool isChannel)
        {
            string message = string.Empty;
            try
            {
                await this._ignoreDao.AddAsync(new TIgnoreEntry
                {
                    GuildId = this.Context.Guild.Id,
                    IsChannel = isChannel,
                    SubjectId = subjectId,
                    SubjectName = subjectName
                });
                message = $"Added `{subjectName}` to the ignore list.";
            }
            catch (AlreadyExistingException)
            {
                message = $"`{subjectName}` is already on the ignore list.";
            }
            catch (NotSavedException)
            {
                message = $"Failed to add `{subjectName}` to the ignore list.";
            }
            return message;
        }

        private async Task SendReplies(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                await this.ReplyAsync(message);
                await this.ReplyAsync(this._random.FromList(this._ignoredReactionList));
            }
        }
    }
}
