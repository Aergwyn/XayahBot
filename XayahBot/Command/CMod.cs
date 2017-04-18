using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Service;
using XayahBot.Utility;
using XayahBot.Command.Attribute;

namespace XayahBot.Command
{
    public class CMod : ModuleBase
    {
        private readonly string _logRequest = "\"{0}\" requested \"mod\" command.";
        private readonly string _logModSuccess = "Promoted \"{0}\".";

        private readonly string _modSuccess = "Promoted `{0}`.";
        private readonly string _modFailed = "Failed to promote `{0}`.";

        //

        [Command("mod")]
        [RequireContext(ContextType.DM)]
        [RequireAdmin]
        [Summary("Promotes a specific user.")]
        public Task Mod(string name)
        {
            string message = string.Empty;
            Logger.Log(LogSeverity.Info, nameof(CMod), string.Format(this._logRequest, this.Context.User));
            if (PermissionService.AddMod(name))
            {
                message = string.Format(this._modSuccess, name);
                Logger.Log(LogSeverity.Warning, nameof(CMod), string.Format(this._logModSuccess, name));
            }
            else
            {
                message = string.Format(this._modFailed, name);
            }
            ReplyAsync(message);
            return Task.CompletedTask;
        }
    }
}
