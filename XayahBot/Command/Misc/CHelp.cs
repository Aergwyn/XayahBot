using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.Remind;
using XayahBot.Extension;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Misc
{
    public class CHelp : ModuleBase
    {
        private static string _cHelpTitle = "General";
        private static string _cAreTitle = "Obligatory 8ball";
        private static string _cRemindTitle = "Reminder for weak brains";
        private static string _cIncidentsTitle = "Incident notifications (via Riot-API)";
        private static string _cDataTitle = "General champion data (via Riot-API)";

        [Command("help")]
        public Task Help([Remainder] string page = "")
        {
            Task.Run(() => this.ProcessHelp(page));
            return Task.CompletedTask;
        }

        private async Task ProcessHelp(string text)
        {
            try
            {
                text = this.TrimText(text);
                IMessageChannel channel = await ChannelProvider.GetDMChannelAsync(this.Context);
                FormattedEmbedBuilder message = new FormattedEmbedBuilder()
                    .AppendTitle("Xayah Bot Help");
                switch (NumberUtil.StripForNumber(text))
                {
                    case 1:
                        this.Append8BallHelp(message);
                        break;
                    case 2:
                        this.AppendRemindHelp(message);
                        break;
                    case 3:
                        this.AppendIncidentsHelp(message);
                        break;
                    case 4:
                        this.AppendChampHelp(message);
                        break;
                    default:
                        this.AppendGeneralHelp(message);
                        break;
                }
                await channel.SendEmbedAsync(message);
                await this.Context.Message.AddReactionIfNotDMAsync(this.Context, XayahReaction.Success);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private string TrimText(string text)
        {
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

        private FormattedEmbedBuilder AppendGeneralHelp(FormattedEmbedBuilder message)
        {
            this.AppendDescriptionTitle(message, _cHelpTitle)
                .AppendDescription("Hello human. If you need help then you came to the right place. I'll tell you how to proceed.")
                .AppendDescriptionNewLine(2)
                .AppendDescription("The help is neatly organized in pages to have a clear view of all possible categories. " +
                    "Just mention me with the corresponding page number and I'll tell you all of the details I think you should know of.")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Usage")
                .AppendDescription("The keyword to this command is `help` followed by an optional page number.")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Examples")
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} help")
                .AppendDescriptionNewLine()
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} help 2")
                .AppendDescriptionNewLine(2)
                .AppendDescription($"Tip: You don't need to mention me in private messages. There are only both of us so I'll know right away what to do.", AppendOption.Italic)
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Help Index")
                .AppendDescription($"`1` - {_cAreTitle}")
                .AppendDescriptionNewLine()
                .AppendDescription($"`2` - {_cRemindTitle}")
                .AppendDescriptionNewLine()
                .AppendDescription($"`3` - {_cIncidentsTitle}")
                .AppendDescriptionNewLine()
                .AppendDescription($"`4` - {_cDataTitle}")
                .AppendDescriptionNewLine()
                .AppendDescription($"`5` - Champion statistics (via ChampionGG-API) [Soon™]")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Contact")
                .AppendDescription("If you still got questions, problems or even suggestions just contact `Aergwyn#8786` and all your desires will be fulfilled " +
                    "(Aergwyn also takes full responsibility, I never said this).")
                .AppendDescriptionNewLine(2)
                .AppendDescription("If that is too direct and you prefer to hide in the shadows (or are just curious) you can just join [this server](https://discord.gg/YhQYAFW) and have a look.");
            return message;
        }

        private FormattedEmbedBuilder Append8BallHelp(FormattedEmbedBuilder message)
        {
            this.AppendDescriptionTitle(message, _cAreTitle)
                .AppendDescription("Every bot needs an 8ball command. A bot without it can't even call itself complete. Also it's the most random and funny command to abuse and spam the chat with.")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Usage")
                .AppendDescription("The keywords to this command are `are`, `is` or `am` followed by a highly creative sentence of yours.")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Examples")
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} are you jealous of Ahri?")
                .AppendDescriptionNewLine()
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} is Riot doing a mistake by not buffing Viktor?")
                .AppendDescriptionNewLine()
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} am I really a challenger level player?")
                .AppendDescriptionNewLine(2)
                .AppendDescription("Tip: There are a few more possible answers than you might expect.", AppendOption.Italic);
            return message;
        }

        private FormattedEmbedBuilder AppendRemindHelp(FormattedEmbedBuilder message)
        {
            string timeUnits = ListUtil.BuildEnumeration(TimeUnit.Values);
            this.AppendDescriptionTitle(message, _cRemindTitle)
                .AppendDescription("If you tend to forget things or just need someone to handle your appointments this is your solution.")
                .AppendDescriptionNewLine(2)
                .AppendDescription($"Currently they are capped at `{Property.RemindDayCap}` days with only `{Property.RemindTextCap}` characters.")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Usage")
                .AppendDescription("This command is split in three parts:")
                .AppendDescriptionNewLine()
                .AppendDescription("- `remind me [number] [time-unit] [text]`; while number and time-unit tell me how long to wait, the text is the message I'll remind you with")
                .AppendDescriptionNewLine()
                .AppendDescription("- `remind me list` shows a list of your active reminders")
                .AppendDescriptionNewLine()
                .AppendDescription("- `remind me clear` if you want to get rid of them")
                .AppendDescriptionNewLine(2)
                .AppendDescription($"Tip: The possible time-units are `{timeUnits}`.", AppendOption.Italic)
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Examples")
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} remind me 2 days finally take the trash out")
                .AppendDescriptionNewLine()
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} remind me list")
                .AppendDescriptionNewLine()
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} remind me clear");
            return message;
        }

        private FormattedEmbedBuilder AppendIncidentsHelp(FormattedEmbedBuilder message)
        {
            this.AppendDescriptionTitle(message, _cIncidentsTitle)
                .AppendDescription("If you want to be up to date about all of League of Legend's troubles, I know a way. You can configure this to post in a channel of your choice.")
                .AppendDescriptionNewLine(2)
                .AppendDescription($"I will check every `{Property.IncidentCheckInterval}` minutes for new or updated incidents.")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Accessibility")
                .AppendDescription("This function is only available in ").AppendDescription("guilds", AppendOption.Bold).AppendDescription("!")
                .AppendDescriptionNewLine()
                .AppendDescription("There will be ").AppendDescription("no", AppendOption.Bold).AppendDescription(" effect in ")
                .AppendDescription("direct messages", AppendOption.Bold).AppendDescription(" or ")
                .AppendDescription("groups", AppendOption.Bold).AppendDescription("!")
                .AppendDescriptionNewLine()
                .AppendDescription("You are required to have the ").AppendDescription("administrator", AppendOption.Bold)
                .AppendDescription(" role to be able to configure the incident notifications!")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Usage")
                .AppendDescription("This command is split in three parts:")
                .AppendDescriptionNewLine()
                .AppendDescription("- `incidents on` followed by a mentioned channel as a post location to activate notifications")
                .AppendDescriptionNewLine()
                .AppendDescription("- `incidents off` to disable notifications")
                .AppendDescriptionNewLine()
                .AppendDescription("- `incidents status` shows the current configuration on your server")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Examples")
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} incidents on #channel_of_your_choice")
                .AppendDescriptionNewLine()
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} incidents off")
                .AppendDescriptionNewLine()
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} incidents status");
            return message;
        }

        private FormattedEmbedBuilder AppendChampHelp(FormattedEmbedBuilder message)
        {
            this.AppendDescriptionTitle(message, _cDataTitle)
                .AppendDescription("Have you ever wondered how far I could throw my Double Daggers? How much base armor Viktor has? How many skins Annie has? " +
                "I at least know the answer to the first and last one. Not enough and way too many, respectively.")
                .AppendDescriptionNewLine(2)
                .AppendDescription("If you need data about a champion just use this command and you'll get an overview about stats, spells and skins.")
                .AppendDescriptionNewLine(2)
                .AppendDescription("Bonus:", AppendOption.Italic)
                .AppendDescription(" If you are unsure how to spell a certain champion name you can ignore special characters and/or whitespaces. " +
                    "If it's even worse (hello Cassiopeia) then I can also handle partial names.")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Usage")
                .AppendDescription("The keyword to this command is `champ` followed by the name I should search for.")
                .AppendDescriptionNewLine(2);
            this.AppendDescriptionTitle(message, "Examples")
                .AppendDescription($"{this.Context.Client.CurrentUser.Mention} champ Xayah");
            return message;
        }
    }
}
