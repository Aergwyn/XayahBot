using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Command.Attribute;
using XayahBot.Database.Model;
using XayahBot.Database.Service;
using XayahBot.Utility;

namespace XayahBot.Command
{
    [Group("list")]
    public class CList : ModuleBase
    {
        private readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";

        private readonly string _emptyModList = "The mod list is empty right now.";
        private readonly string _emptyIgnoreList = "This ignore list is empty right now.";

        //

#pragma warning disable 4014 // Intentional
        [Command("mod")]
        [RequireMod]
        [Summary("Lists all current mods.")]
        public async Task Mod()
        {
            IMessageChannel channel = await this.GetReplyChannel();
            if (channel == null)
            {
                Logger.Log(LogSeverity.Error, nameof(CIgnore), string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string message = string.Empty;
            string[] mods = Property.CfgMods.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            message += $"__List of mods__{Environment.NewLine}```";
            if (mods.Count() > 0)
            {
                for (int i = 0; i < mods.Count(); i++)
                {
                    string mod = mods.ElementAt(i);
                    if (i > 0)
                    {
                        message += Environment.NewLine;
                    }
                    message += mod;
                }
            }
            else
            {
                message += this._emptyModList;
            }
            message += "```";
            channel.SendMessageAsync(message);
        }
#pragma warning restore 4014

#pragma warning disable 4014 // Intentional
        [Command("ignore")]
        [RequireMod]
        [Summary("Lists all ignored use and channel.")]
        public async Task Ignore()
        {
            IMessageChannel channel = await this.GetReplyChannel();
            if (channel == null)
            {
                Logger.Log(LogSeverity.Error, nameof(CIgnore), string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string message = string.Empty;
            string[] userList = Property.CfgIgnore.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<TIgnoredChannel> channelList = IgnoredChannelService.GetChannelList();
            message = $"__Ignored user__{Environment.NewLine}```";
            if (userList.Count() > 0)
            {
                for (int i = 0; i < userList.Count(); i++)
                {
                    if (i > 0)
                    {
                        message += Environment.NewLine;
                    }
                    message += userList.ElementAt(i);
                }
            }
            else
            {
                message += this._emptyIgnoreList;
            }
            message += $"```{Environment.NewLine}__Ignored channel__{Environment.NewLine}```";
            if (channelList.Count > 0)
            {
                for (int i = 0; i < channelList.Count(); i++)
                {
                    if (i > 0)
                    {
                        message += Environment.NewLine;
                    }
                    TIgnoredChannel item = channelList.ElementAt(i);
                    message += $"{item.ChannelId} - {item.ChannelName}";
                }
            }
            else
            {
                message += this._emptyIgnoreList;
            }
            message += "```";
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
