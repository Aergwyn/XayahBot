using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Command.System;
using XayahBot.Error;
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
        public Task Get()
        {
            DiscordFormatMessage message = new DiscordFormatMessage();
            message.Append("List of properties", AppendOption.UNDERSCORE);
            message.AppendCodeBlock(this.BuildPropertyListString());
            this.ReplyAsync(message.ToString());
            return Task.CompletedTask;
        }

        private string BuildPropertyListString()
        {
            string text = string.Empty;
            int maxWidth = Property.UpdatableValues.OrderByDescending(x => x.Name.Length).First().Name.Length;
            for (int i = 0; i < Property.UpdatableValues.Count(); i++)
            {
                if (i > 0)
                {
                    text += Environment.NewLine;
                }
                Property property = Property.UpdatableValues.ElementAt(i);
                text += property.Name.PadRight(maxWidth) + ":" + property.Value;
            }
            return text;
        }

        [Command("set")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        [Summary("Updates a specific property.")]
        public Task Set(string name, [Remainder]string value = "")
        {
            value = value.Trim();
            DiscordFormatMessage message = new DiscordFormatMessage();
            try
            {
                Property property = Property.GetUpdatableByName(name);
                string oldValue = property.Value;
                property.Value = value;
                message.Append($"Property `{property.Name}` changed!");
                message.AppendCodeBlock($"Old:{oldValue}{Environment.NewLine}New:{value}");
            }
            catch (NotExistingException)
            {
                message.Append($"Could not find property with name `{name}`!");
            }
            this.ReplyAsync(message.ToString());
            return Task.CompletedTask;
        }
    }
}
