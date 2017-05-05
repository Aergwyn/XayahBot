#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.System;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Help
{
    public class CHelp : ModuleBase
    {
        private readonly string _finishHelp = "You can also trigger commands with mentioning me instead of the prefix. " +
            "Also always remember using a space between each argument!" + Environment.NewLine +
            "If you have problems, questions and/or suggestions do not hesitate to message {0}.";

        //

        private readonly CommandService _commandService;

        public CHelp(CommandService commandService)
        {
            this._commandService = commandService;
        }

        //

        [Command("help"), Alias("h")]
        [Summary("Displays the list of commands.")]
        public async Task Help()
        {
            IMessageChannel channel = await ResponseHelper.GetDMChannel(this.Context);
            DiscordFormatMessage message = new DiscordFormatMessage();
            message = this.BuildCommonerCommandList(message);
            message = this.BuildModCommandList(message);
            message = this.BuildOwnerCommandList(message);
            message.Append(string.Format(this._finishHelp, Property.Author));
            channel.SendMessageAsync(message.ToString());
        }

        private DiscordFormatMessage BuildCommonerCommandList(DiscordFormatMessage message)
        {
            List<HelpLine> commandList = this.GetMatchingPreconditionList();
            message.Append("Command List", AppendOption.Underscore);
            message.AppendCodeBlock(this.BuildListString(commandList));
            return message;
        }

        private DiscordFormatMessage BuildModCommandList(DiscordFormatMessage message)
        {
            List<HelpLine> commandList = this.GetMatchingPreconditionList(new RequireModAttribute());
            if (DiscordPermissions.IsOwnerOrMod(this.Context) && commandList.Count > 0)
            {
                message.Append("Mod-Commands", AppendOption.Underscore);
                message.AppendCodeBlock(this.BuildListString(commandList));
            }
            return message;
        }

        private DiscordFormatMessage BuildOwnerCommandList(DiscordFormatMessage message)
        {
            List<HelpLine> commandList = this.GetMatchingPreconditionList(new RequireOwnerAttribute());
            if (DiscordPermissions.IsOwner(this.Context) && commandList.Count > 0)
            {
                message.Append("Owner-Commands", AppendOption.Underscore);
                message.AppendCodeBlock(this.BuildListString(commandList));
            }
            return message;
        }

        private List<HelpLine> GetMatchingPreconditionList(PreconditionAttribute precondition = null)
        {
            List<HelpLine> matches = new List<HelpLine>();
            if (precondition != null)
            {
                foreach (CommandInfo command in this._commandService.Commands.Where(x => x.Preconditions.Contains(precondition)))
                {
                    matches.Add(this.GetHelpLine(command));
                }
            }
            else
            {
                foreach (CommandInfo command in this._commandService.Commands)
                {
                    // this is far from optimal but I don't know how to solve it in a more acceptable way right now
                    if (!command.Preconditions.Contains(new RequireOwnerAttribute()) && !command.Preconditions.Contains(new RequireModAttribute()))
                    {
                        matches.Add(this.GetHelpLine(command));
                    }
                }
            }
            return matches;
        }

        private HelpLine GetHelpLine(CommandInfo commandInfo)
        {
            string command = this.BuildAliasString(commandInfo.Aliases);
            foreach (ParameterInfo param in commandInfo.Parameters)
            {
                if (param.IsOptional)
                {
                    command += $" [<{param.Name}>]";
                }
                else
                {
                    command += $" <{param.Name}>";
                }
            }
            return new HelpLine()
            {
                Command = command,
                Summary = commandInfo.Summary
            };
        }

        private string BuildAliasString(IReadOnlyList<string> aliases)
        {
            string result = Property.CmdPrefix.Value;
            Dictionary<int, HashSet<string>> builder = new Dictionary<int, HashSet<string>>();
            foreach (string alias in aliases)
            {
                string[] parts = alias.Split(' ');
                for (int pos = 0; pos < parts.Count(); pos++)
                {
                    if (builder.ContainsKey(pos))
                    {
                        builder[pos].Add(parts.ElementAt(pos));
                    }
                    else
                    {
                        builder.Add(pos, new HashSet<string> { parts.ElementAt(pos) });
                    }
                }
            }
            foreach (HashSet<string> partList in builder.Values)
            {
                result += $"{string.Join("|", partList)} ";
            }
            return result.Trim();
        }

        private string BuildListString(List<HelpLine> helpList)
        {
            string text = string.Empty;
            int maxWidth = helpList.OrderByDescending(x => x.Command.Length).First().Command.Length;
            for (int i = 0; i < helpList.Count; i++)
            {
                if (i > 0)
                {
                    text += Environment.NewLine;
                }
                HelpLine helpLine = helpList.ElementAt(i);
                text += $"{helpLine.Command.PadRight(maxWidth)} - { helpLine.Summary}";
            }
            return text;
        }
    }
}
