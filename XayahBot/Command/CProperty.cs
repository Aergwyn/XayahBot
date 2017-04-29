using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Command.System;
using XayahBot.Utility;

namespace XayahBot.Command
{
    [Group("prop")]
    public class CProperty : LoggedModuleBase
    {
        private readonly string _notFound = "Could not find property with name `{0}`!";
        private readonly string _changed = $"Property `{{0}}` changed!{Environment.NewLine}```{Environment.NewLine}Old:{{1}}{Environment.NewLine}New:{{2}}```";

        //

        [Command("get")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        [Summary("Displays all properties.")]
        public async Task Get()
        {
            string message = $"__List of properties__{Environment.NewLine}```";
            foreach(Property property in Property.UpdatableValues)
            {
                message += $"{Environment.NewLine}{(property.Name + ":").PadRight(this.GetMaxWidth())}\"{property.Value}\"";
            }
            message += "```";
            await this.ReplyAsync(message);
        }

        [Command("set")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        [Summary("Updates a specific property.")]
        public async Task Set(string name, [Remainder]string value = "")
        {
            string message = string.Empty;
            Property property = Property.GetUpdatableByName(name);
            if (property != null)
            {
                string oldValue = property.Value;
                property.Value = value.Trim();
                message = string.Format(this._changed, property.Name, oldValue, property.Value);
            }
            else
            {
                message = string.Format(this._notFound, name);
            }
            await this.ReplyAsync(message);
        }

        private int GetMaxWidth()
        {
            return Property.UpdatableValues.OrderByDescending(x => x.Name.Length).First().Name.Length + 1;
        }
    }
}
