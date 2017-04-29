#pragma warning disable 4014

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
        private readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";

        private readonly string _finishHelp = "You can also trigger commands with mentioning me instead of the prefix. " +
            "Also always remember using a space between each argument!" + Environment.NewLine +
            "If you have problems, questions and/or suggestions do not hesitate to message {0}.";

        //

        private readonly CommandService _commandService;
        private readonly Permission _permission = new Permission();

        public CHelp(CommandService commandService)
        {
            this._commandService = commandService;
        }

        //

        [Command("help"), Alias("h")]
        [Summary("Displays the list of commands.")]
        public async Task Help()
        {
            IMessageChannel channel = null;
            if (this.Context.IsPrivate)
            {
                channel = this.Context.Channel;
            }
            else
            {
                channel = await this.Context.Message.Author.CreateDMChannelAsync();
            }
            if (channel == null)
            {
                Logger.Error(string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string message = string.Empty;
            string prefix = Property.CmdPrefix.Value;
            List<HelpLine> normalCmdList = new List<HelpLine>();
            List<HelpLine> modCmdList = new List<HelpLine>();
            List<HelpLine> ownerCmdList = new List<HelpLine>();
            // Sort commands in permission groups
            foreach (CommandInfo cmd in this._commandService.Commands)
            {
                HelpLine line = this.GetCommandStringSimple(cmd);
                if (cmd.Preconditions.Contains(new RequireOwnerAttribute()))
                {
                    ownerCmdList.Add(line);
                }
                else if (cmd.Preconditions.Contains(new RequireModAttribute()))
                {
                    modCmdList.Add(line);
                }
                else
                {
                    normalCmdList.Add(line);
                }
            }
            message = "__**Command List**__```";
            message += this.GetCommandBlockString(normalCmdList);
            if (this._permission.IsAdminOrMod(this.Context) && modCmdList.Count > 0)
            {
                message += $"```{Environment.NewLine}__Mod-Commands__```";
                message += this.GetCommandBlockString(modCmdList);
            }
            if (this._permission.IsAdmin(this.Context) && ownerCmdList.Count > 0)
            {
                message += $"```{Environment.NewLine}__Owner-Commands__```";
                message += this.GetCommandBlockString(ownerCmdList);
            }
            message += $"```{Environment.NewLine}{string.Format(_finishHelp, Property.Author)}";

            channel.SendMessageAsync(message);
        }

        private HelpLine GetCommandStringSimple(CommandInfo commandInfo)
        {
            string command = this.ListAliases(commandInfo.Aliases);
            foreach (ParameterInfo param in commandInfo.Parameters)
            {
                if (param.IsOptional)
                {
                    //cmd.Aliases.Perm
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

        private string ListAliases(IReadOnlyList<string> aliases)
        {
            string result = Property.CmdPrefix.Value;
            Dictionary<int, HashSet<string>> builder = new Dictionary<int, HashSet<string>>();
            foreach (string alias in aliases)
            {
                string[] parts = alias.Split(' ');
                for (int i = 0; i < parts.Count(); i++)
                {
                    if (builder.ContainsKey(i))
                    {
                        builder[i].Add(parts.ElementAt(i));
                    }
                    else
                    {
                        builder.Add(i, new HashSet<string> { parts.ElementAt(i) });
                    }
                }
            }
            foreach (HashSet<string> partList in builder.Values)
            {
                result += $"{string.Join("|", partList)} ";
            }
            return result.Trim();
        }

        private string GetCommandBlockString(List<HelpLine> helpList)
        {
            string text = string.Empty;
            for (int i = 0; i < helpList.Count; i++)
            {
                HelpLine line = helpList.ElementAt(i);
                text += $"{Environment.NewLine}{line.Command.PadRight(helpList.OrderByDescending(x => x.Command.Length).First().Command.Length)} - {line.Summary}";
            }
            return text;
        }
    }
}
