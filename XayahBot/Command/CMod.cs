using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Service;
using XayahBot.Utility;

namespace XayahBot.Command
{
    public class CMod : ModuleBase
    {
        private static readonly string _logRequest = "\"{0}\" requested \"mod {1}\".";
        private static readonly string _modAddSuccess = "Added `{0}` to list of mods.";
        private static readonly string _logModAddSuccess = "Added \"{0}\" to list of mods.";
        private static readonly string _modAddFailed = "Failed to add `{0}`.";
        private static readonly string _modRemoveSuccess = "Removed `{0}` from list of mods.";
        private static readonly string _logModRemoveSuccess = "Removed \"{0}\" from list of mods.";
        private static readonly string _modRemoveFailed = "Failed to remove `{0}`.";
        private static readonly string _unknownOption = "`{0}` is an unknown option! Use `add` or `remove` instead.";

        //

        [Command("mod")]
        [RequireContext(ContextType.DM)]
        public Task Mod(string option = "", string name = "", [Remainder] string trash = "")
        {
            Logger.Log(LogSeverity.Info, nameof(CMod), string.Format(_logRequest, this.Context.User, option));
            if (option.ToLower().Equals("add"))
            {
                if (PermissionService.AddMod(name))
                {
                    ReplyAsync(string.Format(_modAddSuccess, name));
                    Logger.Log(LogSeverity.Warning, nameof(CMod), string.Format(_logModAddSuccess, name));
                }
                else
                {
                    ReplyAsync(string.Format(_modAddFailed, name));
                }
            }
            else if (option.ToLower().Equals("remove"))
            {
                if (PermissionService.RemoveMod(name))
                {
                    ReplyAsync(string.Format(_modRemoveSuccess, name));
                    Logger.Log(LogSeverity.Info, nameof(CMod), string.Format(_logModRemoveSuccess, name));
                }
                else
                {
                    ReplyAsync(string.Format(_modRemoveFailed, name));
                }
            }
            else
            {
                ReplyAsync(string.Format(_unknownOption, name));
            }
            return Task.CompletedTask;
        }
    }
}
