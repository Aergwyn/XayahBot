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
        public Task Help([Remainder] string page = "")
        {
            Task.Run(() => this.ShowHelp(page));
            return Task.CompletedTask;
        }

        private async Task ShowHelp(string text)
        {
            text = this.TrimText(text);
            FormattedEmbedBuilder message = new FormattedEmbedBuilder()
                .AppendTitle("Xayah Bot Help");
            switch(NumberUtil.StripForNumber(text))
            {
                case 1:
                    this.Append8BallHelp(message);
                    break;
                default:
                    this.AppendGeneralHelp(message);
                    break;
            }
            await this.ReplyAsync(message);
        }

        private string TrimText(string text) {
            text.Trim();
            string[] parts = text.Split(new char[] { ' ' }, 2);
            if (parts.Length > 0)
            {
                text = parts[0].Trim();
            }
            return text;
        }

        private FormattedEmbedBuilder AppendDescriptionTitle(FormattedEmbedBuilder message, string text)
        {
            return message.AppendDescription(text, AppendOption.Underscore).AppendDescriptionNewLine();
        }

        private FormattedEmbedBuilder Append8BallHelp(FormattedEmbedBuilder message)
        {
            this.AppendDescriptionTitle(message, "Obligatory 8ball")
                .AppendDescription("Every bot needs an 8ball command. A bot without it can't even call itself complete. Also it's the most random and funny command to abuse and spam the chat with.")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Usage")
                .AppendDescription("The keywords to this command are `are`, `is` and `am` followed by a highly creative sentence of yours.")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Examples")
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} are you the best?").AppendDescriptionNewLine()
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} is it right that you are the best?").AppendDescriptionNewLine()
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} am I your most loyal servant?").AppendDescriptionNewLine()
                .AppendDescription("Tip: There are a few more possible answers than you might expect.", AppendOption.Italic);
            return message;
        }

        private FormattedEmbedBuilder AppendGeneralHelp(FormattedEmbedBuilder message)
        {
            this.AppendDescriptionTitle(message, "General")
                .AppendDescription("Hello human. If you need help then you came to the right place. I'll tell you how to proceed.")
                .AppendDescriptionNewLine(2)
                .AppendDescription("The help is neatly organized in pages to have a clear view of all possible categories. " +
                    "Just mention me with the corresponding page number and I'll tell you all of the details I think you should know of.")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Usage")
                .AppendDescription("The keyword to this command is `help` followed by an optional page number.")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Examples")
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} help").AppendDescriptionNewLine()
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} help 2").AppendDescriptionNewLine()
                .AppendDescription($"Tip: In private you don't need to mention me. It's only me there so it's kinda obvious...", AppendOption.Italic)
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Help Index")
                .AppendDescription("`1` - Obligatory 8ball").AppendDescriptionNewLine()
                .AppendDescription("`2` - Reminder for weak brains").AppendDescriptionNewLine()
                .AppendDescription("`3` - Incident notifications (via Riot-API)").AppendDescriptionNewLine()
                .AppendDescription("`4` - General champion data (via Riot-API)").AppendDescriptionNewLine()
                .AppendDescription("`5` - Champion statistics (via ChampionGG-API) [Soon™]")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Contact")
                .AppendDescription("If you still got questions, problems or even suggestions just contact `Aergwyn#8786` and all your desires will be fulfilled " +
                    "(Aergwyn also takes full responsibility, I never said this).")
                .AppendDescriptionNewLine(2)
                .AppendDescription("If that is too direct and you prefer to hide in the shadows (or are just curious) you can just join [this server](https://discord.gg/YhQYAFW) and have a look.");
            return message;
        }
    }
}
