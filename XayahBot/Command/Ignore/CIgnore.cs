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
        private readonly string _ignoreSuccess = "Added `{0}` to the ignore list.";
        private readonly string _ignoreFailed = "Failed to add `{0}` to the ignore list.";
        private readonly string _ignoreExisting = "`{0}` is already on the ignore list.";

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
        private readonly IgnoreService _ignoreService = new IgnoreService();

        [Command("ignore")]
        [RequireMod]
        [RequireContext(ContextType.Guild)]
        [Summary("Adds all mentioned user and channel to the ignore list.")]
        public async Task Ignore([Remainder] string text)
        {
            string message = string.Empty;
            foreach (ulong userId in this.Context.Message.MentionedUserIds.Distinct())
            {
                if (this.IsActualUser(userId))
                {
                    IUser user = await this.Context.Guild.GetUserAsync(userId);
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

        private bool IsActualUser(ulong user)
        {
            if (!user.Equals(this.Context.Client.CurrentUser.Id) && !this._permission.IsOwner(this.Context))
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
                await this._ignoreService.AddAsync(new TIgnoreEntry
                {
                    Guild = this.Context.Guild.Id,
                    IsChannel = isChannel,
                    SubjectId = subjectId,
                    SubjectName = subjectName
                });
                message = string.Format(this._ignoreSuccess, subjectName);
            }
            catch (AlreadyExistingException)
            {
                message = string.Format(this._ignoreExisting, subjectName);
            }
            catch (NotSavedException)
            {
                message = string.Format(this._ignoreFailed, subjectName);
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
