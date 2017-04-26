#pragma warning disable 4014

using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Utility;
using System.Collections.Generic;
using System.Linq;
using XayahBot.Command.Attribute;
using XayahBot.Service;
using XayahBot.Database.Service;

namespace XayahBot.Command
{
    public class CHelp : ModuleBase
    {
        public CommandService CmdService { get; set; }

        //

        public CHelp(CommandService cmdService)
        {
            this.CmdService = cmdService;
        }

        //

        private readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";

        private readonly string _finishHelp = "You can also trigger commands with mentioning me instead of the prefix. " +
            "Also always remember using a space between each argument!" + Environment.NewLine +
            "If you have problems, questions and/or suggestions do not hesitate to message {0}.";

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
                Logger.Log(LogSeverity.Error, nameof(CHelp), string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string message = string.Empty;
            string prefix = Property.CmdPrefix.Value;
            List<HelpLine> reducedCmdList = new List<HelpLine>();
            List<HelpLine> normalCmdList = new List<HelpLine>();
            List<HelpLine> modCmdList = new List<HelpLine>();
            List<HelpLine> ownerCmdList = new List<HelpLine>();
            // Sort commands in permission groups
            foreach (CommandInfo cmd in this.CmdService.Commands)
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
                    if (!cmd.Preconditions.Contains(new CheckIgnoredUserAttribute()))
                    {
                        reducedCmdList.Add(line);
                    }
                    normalCmdList.Add(line);
                }
            }
            message = "__**Command List**__```";
            if (IgnoreService.IsIgnored(this.Context.Guild.Id, this.Context.User.Id))
            {
                message += this.GetCommandBlockString(reducedCmdList);
            }
            else
            {
                message += this.GetCommandBlockString(normalCmdList);
            }
            if (PermissionService.IsAdminOrMod(this.Context) && modCmdList.Count > 0)
            {
                message += $"```{Environment.NewLine}__Mod-Commands__```";
                message += this.GetCommandBlockString(modCmdList);
            }
            if (PermissionService.IsAdmin(this.Context) && ownerCmdList.Count > 0)
            {
                message += $"```{Environment.NewLine}__Admin-Commands__```";
                message += this.GetCommandBlockString(ownerCmdList);
            }
            message += $"```{Environment.NewLine}{string.Format(_finishHelp, Property.Author)}";

            channel.SendMessageAsync(message);
        }

        //

        private HelpLine GetCommandStringSimple(CommandInfo cmd)
        {
            string command = this.ListAliases(cmd.Aliases);
            foreach (ParameterInfo param in cmd.Parameters)
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
                Summary = cmd.Summary
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

        private string GetCommandBlockString(List<HelpLine> cmdList)
        {
            string text = string.Empty;
            for (int i = 0; i < cmdList.Count; i++)
            {
                if (i > 0)
                {
                    text += Environment.NewLine;
                }
                HelpLine line = cmdList.ElementAt(i);
                text += $"{line.Command.PadRight(cmdList.OrderByDescending(x => x.Command.Length).First().Command.Length)} - {line.Summary}";
            }
            return text;
        }
    }
}
