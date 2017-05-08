using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Command.Precondition;
using XayahBot.Error;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Owner
{
    [Group("prop")]
    [Category(CategoryType.OWNER)]
    public class CProperty : LoggedModuleBase
    {
        [Command("get")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        [Summary("Displays all properties.")]
        public Task Get()
        {
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .AppendDescription("Here is a list of available properties.")
                .AppendDescriptionCodeBlock(this.BuildPropertyListString());
            this.ReplyAsync("", false, message.ToEmbed());
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
            DiscordFormatEmbed message = new DiscordFormatEmbed();
            try
            {
                Property property = Property.GetUpdatableByName(name);
                string oldValue = property.Value;
                property.Value = value;
                message.AppendDescription($"Value of property named `{property.Name}` changed.")
                    .AppendDescriptionCodeBlock($"Old:{oldValue}{Environment.NewLine}New:{value}");
            }
            catch (NotExistingException)
            {
                message.AppendDescription($"Could not find property named `{name}`!");
            }
            this.ReplyAsync("", false, message.ToEmbed());
            return Task.CompletedTask;
        }
    }
}
