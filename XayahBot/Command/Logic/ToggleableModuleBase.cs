using Discord.Commands;
using XayahBot.Extension;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Logic
{
    public abstract class ToggleableModuleBase : ModuleBase
    {
        protected abstract Property GetDisableProperty();

        protected bool IsDisabled()
        {
            string value = this.GetDisableProperty()?.Value;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            return true;
        }

        protected void NotifyDisabledCommand()
        {
            FormattedEmbedBuilder message = new FormattedEmbedBuilder()
                .CreateFooterIfNotDM(this.Context)
                .AppendTitle($"{XayahReaction.Warning} Command disabled")
                .AppendDescription("This command is disabled because a certain someone is too lazy to fix it.");
            this.ReplyAsync(message);
        }
    }
}
