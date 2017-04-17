using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Utility;
using XayahBot.Command.Attribute;

namespace XayahBot.Command
{
    [Group("get")]
    public class CGet : ModuleBase
    {
        private readonly string _logRequest = "\"{0}\" requested \"{1}\".";

        private readonly string _notFound = "Could not find property with name `{0}`!";
        private readonly string _changed = $"Property changed...{Environment.NewLine}```Old: {{0}}:\"{{1}}\"{Environment.NewLine}New: {{0}}:\"{{2}}\"```";
        private readonly string _currentValue = "```{0}:\"{1}\"```";

        //

        [Command("property"), Alias("p")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        [Summary("Lists all or a specific property.")]
        public Task Property(string name = "")
        {
            Logger.Log(LogSeverity.Debug, nameof(CGet), string.Format(this._logRequest, this.Context.User, "get property"));

            if (!string.IsNullOrWhiteSpace(name))
            {
                Property property = Utility.Property.GetByName(name);
                if (property != null)
                {
                    ReplyAsync(string.Format(this._currentValue, property.Name, property.Value));
                }
                else
                {
                    ReplyAsync(string.Format(this._notFound, name));
                }
            }
            else
            {
                string values = $"__List of properties__{Environment.NewLine}```";
                List<Property> displayList = Utility.Property.Values.Where(x => x.Updatable).ToList();
                for (int i = 0; i < displayList.Count; i++)
                {
                    if (i > 0)
                    {
                        values += Environment.NewLine;
                    }
                    Property property = displayList.ElementAt(i);
                    values += $"{(property.Name + ":").PadRight(Utility.Property.Values.Where(x => x.Updatable).OrderByDescending(x => x.Name.Length).First().Name.Length + 1, ' ')}\"{property.Value}\"";
                }
                values += "```";
                ReplyAsync(values);
            }
            return Task.CompletedTask;
        }
    }
}
