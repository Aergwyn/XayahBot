using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Commands;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Utility
{
    public static class CommandUtil
    {
        public static bool HasRequireRole<T>(CommandInfo command) where T : PreconditionAttribute
        {
            T match = GetPrecondition<T>(command);
            if (match != null)
            {
                return true;
            }
            return false;
        }

        public static T GetPrecondition<T>(CommandInfo command) where T : PreconditionAttribute
        {
            T match = GetPrecondition<T>(command.Preconditions);
            if (match == null)
            {
                match = GetPrecondition<T>(command.Module.Preconditions);
            }
            return match;
        }

        public static T GetPrecondition<T>(IReadOnlyList<PreconditionAttribute> preconditions) where T : PreconditionAttribute
        {
            T match = default(T);
            foreach (PreconditionAttribute attribute in preconditions)
            {
                if (attribute is T)
                {
                    match = attribute as T;
                    break;
                }
            }
            return match;
        }

        public static string ToHelpString(CommandInfo command)
        {
            string commandText = BuildAliasString(command.Aliases);
            string contextText = BuildAllowedContextString(command.Preconditions);
            string summaryText = command.Summary;
            foreach (ParameterInfo param in command.Parameters)
            {
                if (param.IsOptional)
                {
                    commandText += $" [<{param.Name}>]";
                }
                else
                {
                    commandText += $" <{param.Name}>";
                }
            }
            DiscordFormatMessage message = new DiscordFormatMessage()
                .Append(commandText, AppendOption.Bold)
                .Append(Environment.NewLine)
                .Append(contextText, AppendOption.Italic)
                .Append(Environment.NewLine)
                .Append(summaryText);
            return message.ToString();
        }

        private static string BuildAliasString(IReadOnlyList<string> aliases)
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

        private static string BuildAllowedContextString(IReadOnlyList<PreconditionAttribute> preconditions)
        {
            string contexts = "All";
            RequireContextAttribute match = GetPrecondition<RequireContextAttribute>(preconditions);
            if (match != null)
            {
                contexts = match.Contexts.ToString();
            }
            return "Context: " + contexts;
        }
    }
}
