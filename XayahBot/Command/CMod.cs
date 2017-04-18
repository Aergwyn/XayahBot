using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Service;
using XayahBot.Utility;
using XayahBot.Command.Attribute;
using System;
using System.Linq;

namespace XayahBot.Command
{
    [Group("mod")]
    public class CMod : ModuleBase
    {
        private readonly string _logRequest = "\"{0}\" requested \"mod toggle\" command.";
        private readonly string _logAddSuccess = "Added \"{0}\" to the list of mods.";
        private readonly string _logRemoveSuccess = "Removed \"{0}\" from the list of mods.";

        private readonly string _emptyModList = "The mod list is empty right now.";
        private readonly string _addSuccess = "Added `{0}` to the list of mods.";
        private readonly string _removeSuccess = "Removed `{0}` from the list of mods.";
        private readonly string _toggleFailed = "Failed to change mod status of `{0}`.";

        //

        [Command("list")]
        [RequireContext(ContextType.DM)]
        [RequireMod]
        [Summary("Lists all current mods.")]
        public Task List()
        {
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
            ReplyAsync(message);
            return Task.CompletedTask;
        }

        [Command("toggle")]
        [RequireContext(ContextType.DM)]
        [RequireAdmin]
        [Summary("Adds/Removes a specific user to/from the mod list.")]
        public Task Toggle(string name)
        {
            string message = string.Empty;
            Logger.Log(LogSeverity.Info, nameof(CMod), string.Format(this._logRequest, this.Context.User));
            switch (PermissionService.ToggleMod(name))
            {
                case 0:
                    message = string.Format(this._addSuccess, name);
                    Logger.Log(LogSeverity.Warning, nameof(CMod), string.Format(this._logAddSuccess, name));
                    break;
                case 1:
                    message = string.Format(this._removeSuccess, name);
                    Logger.Log(LogSeverity.Warning, nameof(CMod), string.Format(this._logRemoveSuccess, name));
                    break;
                case 2:
                    message = string.Format(this._toggleFailed, name);
                    break;
            }
            ReplyAsync(message);
            return Task.CompletedTask;
        }
    }
}
