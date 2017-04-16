using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Utility;
using System.Collections.Generic;
using System.Linq;
using XayahBot.Command.Attribute;

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

        private static readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";

        private static readonly string _finishHelp = "You can also trigger commands with mentioning me instead of the prefix. " +
            "Also always remember using a space between each argument!" + Environment.NewLine +
            "If you have problems, questions and/or suggestions do not hesitate to message {0}.";

        //

#pragma warning disable 4014 // Intentional
        [Command("help"), Alias("h")]
        [Summary("Displays the list of commands.")]
        public async Task Help(string command = "")
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
                Logger.Log(LogSeverity.Error, nameof(CProperty), string.Format(_logNoReplyChannel, this.Context.User));
                return;
            }
            string message = string.Empty;
            if (string.IsNullOrWhiteSpace(command))
            {
                string prefix = Property.CmdPrefix.Value;
                List<HelpLine> normalCmdList = new List<HelpLine>();
                List<HelpLine> modCmdList = new List<HelpLine>();
                List<HelpLine> adminCmdList = new List<HelpLine>();
                foreach (CommandInfo cmd in this.CmdService.Commands)
                {
                    if (cmd.Preconditions.Contains(new RequireOwnerAttribute()))
                    {
                        adminCmdList.Add(this.GetCommandStringSimple(cmd));
                    }
                    else if (cmd.Preconditions.Contains(new RequireModAttribute()))
                    {
                        modCmdList.Add(this.GetCommandStringSimple(cmd));
                    }
                    else
                    {
                        normalCmdList.Add(this.GetCommandStringSimple(cmd));
                    }
                }
                message = "__**Command List**__```";
                message += this.GetCommandBlockString(normalCmdList);
                message += $"```{Environment.NewLine}__Mod-Commands__```";
                message += this.GetCommandBlockString(modCmdList);
                message += $"```{Environment.NewLine}__Admin-Commands__```";
                message += this.GetCommandBlockString(adminCmdList);
                message += $"```{Environment.NewLine}{string.Format(_finishHelp, Property.Author)}";
            }
            else
            {
                int pos = this.Context.Message.Content.IndexOf(command);
                SearchResult result = this.CmdService.Search(this.Context, pos);
                IReadOnlyList<CommandMatch> matches = result.Commands;
                message = "Not implemented yet.";
                if (matches != null && matches.Count > 0)
                {
                    List<CommandMatch> visibleCommands = new List<CommandMatch>();
                    //check precondition if able to see
                    if (matches.Count > 1)
                    {
                        //TODO multiple commands
                    }
                    else
                    {
                        //TODO detailed info
                        //else
                        //TODO no permission to see this
                    }
                }
                else
                {
                    //TODO no command found
                }
            }
            channel.SendMessageAsync(message);
        }
#pragma warning restore 4014

        //

        private HelpLine GetCommandStringSimple(CommandInfo cmd)
        {
            string command = $".{string.Join("|.", cmd.Aliases)}";
            foreach (ParameterInfo param in cmd.Parameters)
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
                Summary = cmd.Summary
            };
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
