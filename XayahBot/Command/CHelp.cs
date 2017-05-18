#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.Precondition;
using XayahBot.Error;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command
{
    [Category(CategoryType.HELP)]
    public class CHelp : ModuleBase
    {
        private readonly string _finishHelp = "You can also trigger commands by mentioning me instead of the prefix. " +
            Environment.NewLine + "If you have problems, questions and/or suggestions do not hesitate to message {0} or join my [server]({1}).";

        private readonly CommandService _commandService;

        public CHelp(CommandService commandService)
        {
            this._commandService = commandService;
        }

        [Command("help"), Alias("h")]
        [Summary("Displays the list of commands.")]
        public async Task Help([Remainder] string category = "")
        {
            IMessageChannel channel = await ChannelRetriever.GetDMChannelAsync(this.Context);
            Category requestedCategory = Category.GetByName(category);
            if (!this.CategoryContainsAvailableCommands(requestedCategory))
            {
                requestedCategory = Category.Help;
            }
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .AppendDescription($"Here is a list of commands for the category `{requestedCategory}`.")
                .AppendDescription(Environment.NewLine);
            this.ListCommandsForCategory(requestedCategory, message);
            message.AppendDescription(Environment.NewLine + Environment.NewLine);
            this.ListOtherCategories(requestedCategory, message);
            if (requestedCategory.Equals(Category.Help))
            {
                message.AppendDescription(Environment.NewLine + Environment.NewLine)
                    .AppendDescription(string.Format(this._finishHelp, Property.Author, Property.HelpServerLink));
            }
            channel.SendMessageAsync("", false, message.ToEmbed());
        }

        private bool CategoryContainsAvailableCommands(Category category)
        {
            List<CommandInfo> commands = this.GetCommandsOfCategory(category);
            List<CommandInfo> reducedList = new List<CommandInfo>(commands);
            foreach (CommandInfo command in commands)
            {
                if ((CommandUtil.HasRequireRole<RequireOwnerAttribute>(command) && !DiscordPermissions.IsOwner(this.Context)) ||
                    (CommandUtil.HasRequireRole<RequireModAttribute>(command) && !DiscordPermissions.IsOwnerOrMod(this.Context)))
                {
                    reducedList.Remove(command);
                }
            }
            if (reducedList.Count > 0)
            {
                return true;
            }
            return false;
        }

        private void ListCommandsForCategory(Category category, DiscordFormatEmbed message)
        {
            List<CommandInfo> commands = this.GetCommandsOfCategory(category);
            commands = this.ListOwnerCommands(commands, message);
            commands = this.ListModCommands(commands, message);
            foreach (CommandInfo command in commands)
            {
                message.AppendDescription(Environment.NewLine)
                    .AppendDescription(CommandUtil.ToHelpString(command));
            }
        }

        private List<CommandInfo> GetCommandsOfCategory(Category category)
        {
            List<CommandInfo> matches = new List<CommandInfo>();
            foreach (CommandInfo command in this._commandService.Commands)
            {
                if (this.IsCommandOfCategory(command, category))
                {
                    matches.Add(command);
                }
            }
            return matches;
        }

        private bool IsCommandOfCategory(CommandInfo command, Category category)
        {
            CategoryAttribute match = CommandUtil.GetPrecondition<CategoryAttribute>(command);
            if (match != null && match.CategoryType.Equals(category.CategoryType))
            {
                return true;
            }
            return false;
        }

        private List<CommandInfo> ListOwnerCommands(List<CommandInfo> commands, DiscordFormatEmbed message)
        {
            List<CommandInfo> reducedList = new List<CommandInfo>(commands);
            foreach (CommandInfo command in commands)
            {
                if (CommandUtil.HasRequireRole<RequireOwnerAttribute>(command))
                {
                    if (DiscordPermissions.IsOwner(this.Context))
                    {
                        message.AppendDescription(Environment.NewLine)
                            .AppendDescription(CommandUtil.ToHelpString(command));
                    }
                    reducedList.Remove(command);
                }
            }
            return reducedList;
        }

        private List<CommandInfo> ListModCommands(List<CommandInfo> commands, DiscordFormatEmbed message)
        {
            List<CommandInfo> reducedList = new List<CommandInfo>(commands);
            foreach (CommandInfo command in commands)
            {
                if (CommandUtil.HasRequireRole<RequireModAttribute>(command))
                {
                    if (DiscordPermissions.IsOwnerOrMod(this.Context))
                    {
                        message.AppendDescription(Environment.NewLine)
                            .AppendDescription(CommandUtil.ToHelpString(command));
                    }
                    reducedList.Remove(command);
                }
            }
            return reducedList;
        }

        private void ListOtherCategories(Category category, DiscordFormatEmbed message)
        {
            message.AppendDescription("Other categories", AppendOption.Bold);
            foreach (Category otherCategory in Category.Values.Where(x => !x.Equals(category)))
            {
                if (this.CategoryContainsAvailableCommands(otherCategory))
                {
                    message.AppendDescription(Environment.NewLine)
                        .AppendDescription(otherCategory.ToString());
                }
            }
        }
    }
}
