using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Extension;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Misc
{
    public class CHelp : ModuleBase
    {
        [Command("help")]
        public Task Help([Remainder] string trash = "")
        {
            Task.Run(() => this.ShowHelp());
            return Task.CompletedTask;
        }

        private async Task ShowHelp()
        {
            FormattedEmbedBuilder message = new FormattedEmbedBuilder()
                .AppendDescription("This is a test message.");
            IUserMessage postedMessage = await this.ReplyAsync(message);
            await postedMessage.AddReactionAsync(XayahReaction.LeftArrow);
            await postedMessage.AddReactionAsync(XayahReaction.RightArrow);
        }
    }
}
