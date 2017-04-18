using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly string _logRequestChannel = "\"{0}\" requested \"ignore channel\" command.";
        private readonly string _logToggleSuccess = "Toggled status of \"{0}\" on the ignore list.";
        private readonly string _logRequestUser = "\"{0}\" requested \"ignore user\" command.";
        private readonly string _logAddSuccess = "Added \"{0}\" to the ignore list.";
        private readonly string _logRemoveSuccess = "Removed \"{0}\" from the ignore list.";

        private readonly string _emptyIgnoreList = "This ignore list is empty right now.";
        private readonly string _multipleMentions = "You mentioned multiple channel to ignore. Try that one at a time.";
        private readonly string _toggleSuccess = "Toggled status of `{0}` on the ignore list.";
        private readonly string _noMention = "You have to mention a channel to ignore. Duh.";
        private readonly string _addSuccess = "Added `{0}` to the ignore list.";
        private readonly string _removeSuccess = "Removed `{0}` from the ignore list.";
        private readonly string _toggleFailed = "Failed to change ignore status of `{0}`.";

        //

#pragma warning disable 4014 // Intentional
        [Command("list")]
        [RequireMod]
        [Summary("Lists all ignored channel and user.")]
        public async Task List()
        {
            IMessageChannel replyChannel = await this.GetReplyChannel();
            if (replyChannel == null)
            {
                Logger.Log(LogSeverity.Error, nameof(CIgnore), string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string message = string.Empty;
            string[] userList = Property.CfgMods.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<TIgnoredChannel> channelList = new List<TIgnoredChannel>();
            message = $"__Ignored user__{Environment.NewLine}```";
            if (userList.Count() > 0)
            {
                for (int i = 0; i < userList.Count(); i++)
                {
                    if (i > 0)
                    {
                        message += Environment.NewLine;
                    }
                    string user = userList.ElementAt(i);
                    message += user;
                }
            }
            else
            {
                message += this._emptyIgnoreList;
            }
            message += $"```__Ignored channel__{Environment.NewLine}```";
            if (channelList.Count > 0)
            {
                for (int i = 0; i < channelList.Count(); i++)
                {
                    if (i > 0)
                    {
                        message += Environment.NewLine;
                    }
                    string channel = userList.ElementAt(i);
                    message += channel;
                }
            }
            else
            {
                message += this._emptyIgnoreList;
            }
            message += "```";
            replyChannel.SendMessageAsync(message);
        }
#pragma warning restore 4014

#pragma warning disable 4014 // Intentional
        [Command("channel"), Alias("c")]
        [RequireMod]
        [RequireContext(ContextType.Guild)]
        [Summary("Adds/Removes a specific (mentioned) channel to/from the ignore list.")]
        public async Task Channel([Remainder] string text)
        {
            IMessageChannel replyChannel = await this.GetReplyChannel();
            if (replyChannel == null)
            {
                Logger.Log(LogSeverity.Error, nameof(CIgnore), string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string message = string.Empty;
            Logger.Log(LogSeverity.Info, nameof(CIgnore), string.Format(this._logRequestChannel, this.Context.User));
            if (this.Context.Message.MentionedChannelIds.Count > 0)
            {
                if (this.Context.Message.MentionedChannelIds.Count > 1)
                {
                    message = this._multipleMentions;
                }
                else
                {
                    ulong channel = this.Context.Message.MentionedChannelIds.First();
                    IgnoredChannelService.ToggleChannelAsync(this.Context.Guild.Id, channel);
                    message = string.Format(this._toggleSuccess, channel);
                    Logger.Log(LogSeverity.Warning, nameof(CIgnore), string.Format(this._logToggleSuccess, channel));
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
        [Summary("Adds/Removes a specific user to/from the ignore list.")]
        public async Task User(string user)
        {
            IMessageChannel channel = await this.GetReplyChannel();
            if (channel == null)
            {
                Logger.Log(LogSeverity.Error, nameof(CIgnore), string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string message = string.Empty;
            Logger.Log(LogSeverity.Info, nameof(CIgnore), string.Format(this._logRequestUser, this.Context.User));
            switch (PermissionService.ToggleMod(user))
            {
                case 0:
                    message = string.Format(this._addSuccess, user);
                    Logger.Log(LogSeverity.Warning, nameof(CIgnore), string.Format(this._logAddSuccess, user));
                    break;
                case 1:
                    message = string.Format(this._removeSuccess, user);
                    Logger.Log(LogSeverity.Warning, nameof(CIgnore), string.Format(this._logRemoveSuccess, user));
                    break;
                case 2:
                    message = string.Format(this._toggleFailed, user);
                    break;
            }
            channel.SendMessageAsync(message);
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
