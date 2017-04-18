using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XayahBot.Command.Attribute;
using XayahBot.Database.Model;
using XayahBot.Database.Service;
using XayahBot.Service;
using XayahBot.Utility;

namespace XayahBot.Command
{
    [Group("ignore")]
    public class CIgnore : ModuleBase
    {
        private readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";
        private readonly string _logRequest = "\"{0}\" requested \"ignore {1}\" command.";
        private readonly string _logIgnoreSuccess = "Added \"{0}\" to the ignore list.";

        private readonly string _ignoreSuccess = "Added `{0}` to the ignore list.";
        private readonly string _ignoreFailed = "Failed to add `{0}` to the ignore list.";
        private readonly string _noMention = "You need to mention a channel that you want to ignore...";

        private readonly List<string> _ignoredReactionList = new List<string>
        {
            "I warned you. Oh wait... did I? Crap. My fault.",
            "All out of second chances, sorry!",
            "And they won't be needing that anymore.",
            "My last nerve is long gone."
        };

        //

#pragma warning disable 4014 // Intentional
        [Command("channel"), Alias("c")]
        [RequireMod]
        [RequireContext(ContextType.Guild)]
        [Summary("Ignores commands from a specific (mentioned) channel.")]
        public async Task Channel(string mention)
        {
            IMessageChannel replyChannel = await this.GetReplyChannel();
            if (replyChannel == null)
            {
                Logger.Log(LogSeverity.Error, nameof(CIgnore), string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string message = string.Empty;
            Logger.Log(LogSeverity.Info, nameof(CIgnore), string.Format(this._logRequest, this.Context.User, "channel"));
            if (this.Context.Message.MentionedChannelIds.Count > 0)
            {
                ulong channelId = this.Context.Message.MentionedChannelIds.First();
                string resolvedMessage = this.Context.Message.Resolve();
                int index = resolvedMessage.IndexOf('#');
                string channelName = resolvedMessage.Substring(index);
                if (await IgnoredChannelService.AddChannelAsync(this.Context.Guild.Id, channelId, channelName))
                {
                    message = string.Format(this._ignoreSuccess, channelName);
                    Logger.Log(LogSeverity.Warning, nameof(CIgnore), string.Format(this._logIgnoreSuccess, channelName));
                }
                else
                {
                    message = string.Format(this._ignoreFailed, channelName);
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
        [Summary("Ignores commands from a specific user.")]
        public async Task User(string user)
        {
            IMessageChannel channel = await this.GetReplyChannel();
            if (channel == null)
            {
                Logger.Log(LogSeverity.Error, nameof(CIgnore), string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string message = string.Empty;
            string reaction = string.Empty;
            Logger.Log(LogSeverity.Info, nameof(CIgnore), string.Format(this._logRequest, this.Context.User, "user"));
            if (PermissionService.AddIgnore(user))
            {
                message = string.Format(this._ignoreSuccess, user);
                Logger.Log(LogSeverity.Warning, nameof(CIgnore), string.Format(this._logIgnoreSuccess, user));
                reaction = this._ignoredReactionList.ElementAt(RNG.Next(this._ignoredReactionList.Count) - 1);
            }
            else
            {
                message = string.Format(this._ignoreFailed, user);
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
