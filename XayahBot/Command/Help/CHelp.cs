using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Command.System;
using XayahBot.Utility;

namespace XayahBot.Command.Help
{
    public class CHelp : ModuleBase
    {
        private readonly string _finishHelp = "You can also trigger commands with mentioning me instead of the prefix. " +
            "Also always remember using a space between each argument!" + Environment.NewLine +
            "If you have problems, questions and/or suggestions do not hesitate to message {0}.";

        //

        private readonly CommandService _commandService;
        private readonly Permission _permission = new Permission();
        private readonly ResponseHelper _responseHelper = new ResponseHelper();

        public CHelp(CommandService commandService)
        {
            this._commandService = commandService;
        }

        //

        [Command("help"), Alias("h")]
        [Summary("Displays the list of commands.")]
        public async Task Help()
        {
            IMessageChannel channel = await this._responseHelper.GetDMChannel(this.Context);
            string message = string.Empty;
            List<HelpLine> normalCmdList = new List<HelpLine>();
            List<HelpLine> modCmdList = new List<HelpLine>();
            List<HelpLine> ownerCmdList = new List<HelpLine>();
            foreach (CommandInfo command in this._commandService.Commands)
            {
                HelpLine helpLine = this.GetHelpLine(command);
                if (command.Preconditions.Contains(new RequireOwnerAttribute()))
                {
                    ownerCmdList.Add(helpLine);
                }
                else if (command.Preconditions.Contains(new RequireModAttribute()))
                {
                    modCmdList.Add(helpLine);
                }
                else
                {
                    normalCmdList.Add(helpLine);
                }
            }
            message = "__**Command List**__```";
            message += this.BuildListString(normalCmdList);
            if (this._permission.IsOwnerOrMod(this.Context) && modCmdList.Count > 0)
            {
                message += $"```{Environment.NewLine}__Mod-Commands__```";
                message += this.BuildListString(modCmdList);
            }
            if (this._permission.IsOwner(this.Context) && ownerCmdList.Count > 0)
            {
                message += $"```{Environment.NewLine}__Owner-Commands__```";
                message += this.BuildListString(ownerCmdList);
            }
            message += $"```{Environment.NewLine}{string.Format(_finishHelp, Property.Author)}";
            await channel.SendMessageAsync(message);
        }

        private HelpLine GetHelpLine(CommandInfo commandInfo)
        {
            string command = this.BuildAliasList(commandInfo.Aliases);
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

        private string BuildAliasList(IReadOnlyList<string> aliases)
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
            foreach (HelpLine helpLine in helpList)
            {
                text += $"{Environment.NewLine}{helpLine.Command.PadRight(this.GetMaxWidth(helpList))} - {helpLine.Summary}";
            }
            return text;
        }

        private int GetMaxWidth(List<HelpLine> helpList)
        {
            return helpList.OrderByDescending(x => x.Command.Length).First().Command.Length;
        }
    }
}
