using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using XayahBot.Command.Attribute;
using XayahBot.Service;
using XayahBot.Utility;

namespace XayahBot.Command
{
    public class CUnmod : ModuleBase
    {
        private readonly string _logRequest = "\"{0}\" requested \"unmod\" command.";
        private readonly string _logUnmodSuccess = "Demoted \"{0}\".";

        private readonly string _unmodSuccess = "Demoted `{0}`.";
        private readonly string _unmodFailed = "Failed to demote `{0}`.";

        //

        [Command("unmod")]
        [RequireContext(ContextType.DM)]
        [RequireAdmin]
        [Summary("Demotes a specific user.")]
        public Task Unmod(string name)
        {
            string message = string.Empty;
            Logger.Log(LogSeverity.Info, nameof(CUnmod), string.Format(this._logRequest, this.Context.User));
            if (PermissionService.RemoveMod(name))
            {
                message = string.Format(this._unmodSuccess, name);
                Logger.Log(LogSeverity.Warning, nameof(CUnmod), string.Format(this._logUnmodSuccess, name));
            }
            else
            {
                message = string.Format(this._unmodFailed, name);
            }
            ReplyAsync(message);
            return Task.CompletedTask;
        }
    }
}
