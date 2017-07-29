using Discord.Commands;
using XayahBot.Utility;

namespace XayahBot.Command
{
    public class LoggedModuleBase : ModuleBase
    {
        private readonly string _requestText = "\"{0}\" requested \"{1}\".";

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);
            Logger.Warning(string.Format(this._requestText, this.Context.User, this.Context.Message.Resolve()));
        }
    }
}
