﻿using System;
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

        private readonly RNG _random = new RNG();
        private readonly Permission _permission = new Permission();
        private readonly IgnoreService _ignoreService = new IgnoreService();

        [Command("unignore")]
        [RequireMod]
        [RequireContext(ContextType.Guild)]
        [Summary("Removes all mentioned user and channel from the ignore list.")]
        public async Task Unignore([Remainder] string text)
        {
            string message = string.Empty;
            foreach (ulong userId in this.Context.Message.MentionedUserIds.Distinct())
            {
                if (this.IsActualUser(userId))
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

        private async Task<string> RemoveIgnore(ulong subjectId, string subjectName)
        {
            string message = string.Empty;
            try
            {
                await this._ignoreService.RemoveAsync(this.Context.Guild.Id, subjectId);
                message = string.Format(this._unignoreSuccess, subjectName);
            }
            catch (NotExistingException)
            {
                message = string.Format(this._unignoreNotExisting, subjectName);
            }
            catch (NotSavedException)
            {
                message = string.Format(this._unignoreFailed, subjectName);
            }
            return message;
        }

        private async Task SendReplies(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                await this.ReplyAsync(message);
                await this.ReplyAsync(this._random.FromList(this._unignoredReactionList));
            }
        }
    }
}
