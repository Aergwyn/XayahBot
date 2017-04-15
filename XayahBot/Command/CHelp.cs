using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Service;
using XayahBot.Utility;

namespace XayahBot.Command
{
    public class CHelp : ModuleBase
    {
        private static readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";

        private static readonly string _normalHelp = "__Commands__```" +
            "{0}help, {0}h      - for a detailed list of commands." + Environment.NewLine +
            "{0}what <text>?  - for an 8ball-like response." + Environment.NewLine +
            "{0}info <text>   - for detailed data about a champion sponsored by Riot API." + Environment.NewLine +
            "{0}quiz          - for a random question about a champion sponsored by Riot API." + Environment.NewLine +
            "{0}answer <text> - to answer a previously stated question.```";
        private static readonly string _modHelp = $"{Environment.NewLine}__Mod-Commands__```" +
            "{0}set game <text> - to change my \"status\" message.```";
        private static readonly string _adminHelp = $"{Environment.NewLine}__Admin-Commands__```" +
            "{0}get property [<name>] - for a list of properties [or a specific one] with their respective values." + Environment.NewLine +
            "{0}set property <name>   - to set the value of a property." + Environment.NewLine +
            "{0}mod add|remove <name> - to edit the list of mods (easier than setting the property manually).```";
        private static readonly string _finishHelp = $"{Environment.NewLine}You can also trigger commands with mentioning me instead of the prefix. " +
            "Also always remember using a space between each argument!" + Environment.NewLine +
            "If you have problems and/or questions do not hesitate to message {0}.";

        //

#pragma warning disable 4014 // Intentional
        [Command("help"), Alias("h")]
        public async Task Help([Remainder] string trash = "")
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
            if (channel == null)
            {
                Logger.Log(LogSeverity.Error, nameof(CProperty), string.Format(_logNoReplyChannel, this.Context.User));
                return;
            }
            string prefix = Property.CmdPrefix.Value;
            string message = string.Format(_normalHelp, prefix);
            if (PermissionService.IsAdminOrMod(this.Context))
            {
                message += string.Format(_modHelp, prefix);
            }
            if (PermissionService.IsAdmin(this.Context))
            {
                message += string.Format(_adminHelp, prefix);
            }
            message += string.Format(_finishHelp, Property.Author);
            channel.SendMessageAsync(message);
        }
#pragma warning restore 4014
    }
}
