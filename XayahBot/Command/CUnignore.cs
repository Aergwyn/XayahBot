using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Command.Attribute;
using XayahBot.Database.Service;
using XayahBot.Utility;

namespace XayahBot.Command
{
    public class CUnignore : ModuleBase
    {
        private readonly string _logRequest = "\"{0}\" requested \"unignore\" command.";
        private readonly string _logUnignoreSuccess = "Removed \"{0}\" from the ignore list.";

        private readonly string _unignoreSuccess = "Removed `{0}` from the ignore list.";
        private readonly string _unignoreFailed = "Failed to remove `{0}` from the ignore list.";
        private readonly string _unignoreNotExisting = "`{0}` was never on the ignore list.";

        private readonly List<string> _unignoredReactionList = new List<string>
        {
            "Maybe I over did it.",
            "Well, that's just an invasion of privacy!",
            "Don't embarass me!",
            "But is it really okay?"
        };

        //

#pragma warning disable 4014 // Intentional
        [Command("unignore")]
        [RequireMod]
        [RequireContext(ContextType.Guild)]
        [Summary("Removes all mentioned user and channel from the ignore list.")]
        public async Task Channel([Remainder] string text)
        {
            string message = string.Empty;
            Logger.Log(LogSeverity.Info, nameof(CUnignore), string.Format(this._logRequest, this.Context.User));
            foreach (ulong userId in this.Context.Message.MentionedUserIds.Distinct())
            {
                if (!userId.Equals(this.Context.Client.CurrentUser.Id))
                {
                    IUser user = await this.Context.Guild.GetUserAsync(userId);
                    message += await RemoveIgnore(user.Id, user.ToString()) + Environment.NewLine;
                }
            }
            foreach (ulong channelId in this.Context.Message.MentionedChannelIds.Distinct())
            {
                IChannel channel = await this.Context.Guild.GetChannelAsync(channelId);
                message += await RemoveIgnore(channel.Id, channel.Name) + Environment.NewLine;
            }
            ReplyAsync(message);
            if (!string.IsNullOrWhiteSpace(message))
            {
                ReplyAsync(RNG.FromList(this._unignoredReactionList));
            }
        }
#pragma warning restore 4014

        //

#pragma warning disable 4014 // Intentional
        private async Task<string> RemoveIgnore(ulong subjectId, string subjectName)
        {
            string message = string.Empty;
            switch (await IgnoreService.RemoveAsync(this.Context.Guild.Id, subjectId))
            {
                case 0:
                    message = string.Format(this._unignoreSuccess, subjectName);
                    Logger.Log(LogSeverity.Warning, nameof(CUnignore), string.Format(this._logUnignoreSuccess, subjectName));
                    break;
                case 1:
                    message = string.Format(this._unignoreFailed, subjectName);
                    break;
                case 2:
                    message = string.Format(this._unignoreNotExisting, subjectName);
                    break;
            }
            return message;
        }
#pragma warning restore 4014
    }
}
