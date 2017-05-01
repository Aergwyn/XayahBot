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
        [Command("get")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        [Summary("Displays all properties.")]
        public async Task Get()
        {
            DiscordFormatMessage message = new DiscordFormatMessage();
            message.Append("List of properties", AppendOption.UNDERSCORE);
            message.AppendCodeBlock(this.BuildPropertyListString());
            await this.ReplyAsync(message.ToString());
        }

        private string BuildPropertyListString() {
            string text = string.Empty;
            int maxWidth = Property.UpdatableValues.OrderByDescending(x => x.Name.Length).First().Name.Length + 1;
            for (int i = 0; i < Property.UpdatableValues.Count(); i++)
            {
                if (i > 0)
                {
                    text += Environment.NewLine;
                }
                Property property = Property.UpdatableValues.ElementAt(i);
                text += (property.Name + ":").PadRight(maxWidth) + property.Value;
            }
            return text;
        }

        [Command("set")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        [Summary("Updates a specific property.")]
        public async Task Set(string name, [Remainder]string value = "")
        {
            Property property = Property.GetUpdatableByName(name);
            DiscordFormatMessage message = new DiscordFormatMessage();
            if (property != null)
            {
                string oldValue = property.Value;
                property.Value = value = value.Trim();
                message.Append($"Property `{property.Name}` changed!");
                message.AppendCodeBlock($"Old:{oldValue}{Environment.NewLine}New:{value}");
            }
            else
            {
                message.Append($"Could not find property with name `{name}`!");
            }
            await this.ReplyAsync(message.ToString());
        }
    }
}
