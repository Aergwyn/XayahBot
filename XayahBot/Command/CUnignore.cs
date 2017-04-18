using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XayahBot.Command.Attribute;
using XayahBot.Database.Service;
using XayahBot.Service;
using XayahBot.Utility;

namespace XayahBot.Command
{
    [Group("unignore")]
    public class CUnignore : ModuleBase
    {
        private readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";
        private readonly string _logRequest = "\"{0}\" requested \"unignore {1}\" command.";
        private readonly string _logUnignoreSuccess = "Removed \"{0}\" from the ignore list.";

        private readonly string _unignoreSuccess = "Removed `{0}` from the ignore list.";
        private readonly string _unignoreFailed = "Failed to remove `{0}` from the ignore list.";
        private readonly string _noMention = "You need to mention a channel that you want to unignore...";

        private readonly List<string> _unignoredReactionList = new List<string>
        {
            "Maybe I over did it.",
            "Well, that's just an invasion of privacy!",
            "Humans... they’re so very blind."
        };

        //

#pragma warning disable 4014 // Intentional
        [Command("channel"), Alias("c")]
        [RequireMod]
        [RequireContext(ContextType.Guild)]
        [Summary("Unignores commands from a specific (mentioned) channel.")]
        public async Task Channel(string mention)
        {
            IMessageChannel replyChannel = await this.GetReplyChannel();
            if (replyChannel == null)
            {
                Logger.Log(LogSeverity.Error, nameof(CUnignore), string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string message = string.Empty;
            Logger.Log(LogSeverity.Info, nameof(CUnignore), string.Format(this._logRequest, this.Context.User, "channel"));
            if (this.Context.Message.MentionedChannelIds.Count > 0)
            {
                ulong channelId = this.Context.Message.MentionedChannelIds.First();
                string resolvedMessage = this.Context.Message.Resolve();
                int index = resolvedMessage.IndexOf('#');
                string channelName = resolvedMessage.Substring(index);
                if (await IgnoredChannelService.RemoveChannelAsync(this.Context.Guild.Id, channelId))
                {
                    message = string.Format(this._unignoreSuccess, channelName);
                    Logger.Log(LogSeverity.Warning, nameof(CUnignore), string.Format(this._logUnignoreSuccess, channelName));
                }
                else
                {
                    message = string.Format(this._unignoreFailed, channelName);
                }
            }
            else
            {
                message = this._noMention;
            }
            replyChannel.SendMessageAsync(message);
        }
#pragma warning restore 4014

#pragma warning disable 4014 // Intentional
        [Command("user"), Alias("u")]
        [RequireMod]
        [Summary("Unignores commands from a specific user.")]
        public async Task User(string user)
        {
            IMessageChannel channel = await this.GetReplyChannel();
            if (channel == null)
            {
                Logger.Log(LogSeverity.Error, nameof(CUnignore), string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string message = string.Empty;
            string reaction = string.Empty;
            Logger.Log(LogSeverity.Info, nameof(CUnignore), string.Format(this._logRequest, this.Context.User, "user"));
            if (PermissionService.RemoveIgnore(user))
            {
                message = string.Format(this._unignoreSuccess, user);
                Logger.Log(LogSeverity.Warning, nameof(CUnignore), string.Format(this._logUnignoreSuccess, user));
                reaction = this._unignoredReactionList.ElementAt(RNG.Next(this._unignoredReactionList.Count) - 1);
            }
            else
            {
                message = string.Format(this._unignoreFailed, user);
            }
            channel.SendMessageAsync(message);
            if (!this.Context.IsPrivate && !string.IsNullOrWhiteSpace(reaction))
            {
                ReplyAsync(reaction);
            }
        }
#pragma warning restore 4014

        //

        private async Task<IMessageChannel> GetReplyChannel()
        {
            IMessageChannel channel = null;
            if (this.Context.IsPrivate)
            {
                channel = this.Context.Channel;
            }
            else
            {
                channel = await this.Context.Message.Author.CreateDMChannelAsync();
            }
            return channel;
        }
    }
}
