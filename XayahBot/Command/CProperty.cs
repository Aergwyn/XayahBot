using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Utility;

namespace XayahBot.Command
{
    [Group("prop")]
    public class CProperty : ModuleBase
    {
        private readonly string _logRequest = "\"{0}\" requested \"property set\".";
        private readonly string _logChanged = "\"{0}\" changed to \"{1}\".";

        private readonly string _notFound = "Could not find property with name `{0}`!";
        private readonly string _changed = $"Property changed...{Environment.NewLine}```Old: {{0}}:\"{{1}}\"{Environment.NewLine}New: {{0}}:\"{{2}}\"```";

        //

        [Command("get")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        [Summary("Displays all properties.")]
        public Task Get()
        {
            List<Property> displayList = Utility.Property.Values.Where(x => x.Updatable).ToList();
            string message = $"__List of properties__{Environment.NewLine}```";
            for (int i = 0; i < displayList.Count; i++)
            {
                if (i > 0)
                {
                    message += Environment.NewLine;
                }
                Property property = displayList.ElementAt(i);
                message += $"{(property.Name + ":").PadRight(Utility.Property.Values.Where(x => x.Updatable).OrderByDescending(x => x.Name.Length).First().Name.Length + 1, ' ')}\"{property.Value}\"";
            }
            message += "```";
            ReplyAsync(message);
            return Task.CompletedTask;
        }

        //

        [Command("set")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        [Summary("Updates a specific property.")]
        public Task Property(string name, [Remainder]string value = "")
        {
            string message = string.Empty;
            Logger.Log(LogSeverity.Warning, nameof(CProperty), string.Format(this._logRequest, this.Context.User));
            Property property = Utility.Property.GetByName(name);
            if (property != null)
            {
                string oldValue = property.Value;
                property.Value = value.Trim();
                message = string.Format(this._changed, property.Name, oldValue, property.Value);
                Logger.Log(LogSeverity.Warning, nameof(CProperty), string.Format(this._logChanged, property.Name, property.Value));
            }
            else
            {
                message = string.Format(this._notFound, name);
            }
            ReplyAsync(message);
            return Task.CompletedTask;
        }
    }
}
