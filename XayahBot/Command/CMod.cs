using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Service;
using XayahBot.Utility;
using XayahBot.Command.Attribute;

namespace XayahBot.Command
{
    [Group("mod")]
    public class CMod : ModuleBase
    {
        private readonly string _logRequest = "\"{0}\" requested \"{1}\".";
        private readonly string _modAddSuccess = "Added `{0}` to list of mods.";
        private readonly string _logModAddSuccess = "Added \"{0}\" to list of mods.";
        private readonly string _modAddFailed = "Failed to add `{0}`.";
        private readonly string _modRemoveSuccess = "Removed `{0}` from list of mods.";
        private readonly string _logModRemoveSuccess = "Removed \"{0}\" from list of mods.";
        private readonly string _modRemoveFailed = "Failed to remove `{0}`.";

        //

        [Command("add")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        [Summary("Adds specified person to the mod list.")]
        public Task Add(string name)
        {
            Logger.Log(LogSeverity.Info, nameof(CMod), string.Format(this._logRequest, this.Context.User, "mod add"));
            if (PermissionService.AddMod(name))
            {
                ReplyAsync(string.Format(this._modAddSuccess, name));
                Logger.Log(LogSeverity.Warning, nameof(CMod), string.Format(this._logModAddSuccess, name));
            }
            else
            {
                ReplyAsync(string.Format(this._modAddFailed, name));
            }
            return Task.CompletedTask;
        }

        [Command("remove")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        [Summary("Removes specified person from the mod list.")]
        public Task Remove(string name)
        {
            Logger.Log(LogSeverity.Info, nameof(CMod), string.Format(this._logRequest, this.Context.User, "mod remove"));
            if (PermissionService.RemoveMod(name))
            {
                ReplyAsync(string.Format(this._modRemoveSuccess, name));
                Logger.Log(LogSeverity.Info, nameof(CMod), string.Format(this._logModRemoveSuccess, name));
            }
            else
            {
                ReplyAsync(string.Format(this._modRemoveFailed, name));
            }
            return Task.CompletedTask;
        }
    }
}
