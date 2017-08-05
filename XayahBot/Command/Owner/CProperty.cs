using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Error;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Owner
{
    [Group("prop")]
    public class CProperty : ModuleBase
    {
        [Command("get")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        public Task Get()
        {
            Task.Run(() => this.BuildPropertyList());
            return Task.CompletedTask;
        }

        private async Task BuildPropertyList()
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
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .AppendTitle($"{XayahReaction.Option} Property list")
                .AppendDescription(text, AppendOption.Codeblock);
            await this.ReplyAsync("", false, message.ToEmbed());
        }

        [Command("set")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        public Task Set(string name, [Remainder]string value = "")
        {
            Task.Run(() => this.SetProperty(name, value));
            return Task.CompletedTask;
        }

        private async Task SetProperty(string name, string value)
        {
            value = value.Trim();
            DiscordFormatEmbed message = new DiscordFormatEmbed();
            try
            {
                Property property = Property.GetUpdatableByName(name);
                string oldValue = property.Value;
                property.Value = value;
                message
                    .AppendTitle($"{XayahReaction.Success} Done")
                    .AppendDescription($"Old:{oldValue}{Environment.NewLine}New:{value}", AppendOption.Codeblock);
            }
            catch (NotExistingException)
            {
                message
                    .AppendTitle($"{XayahReaction.Error} Nope")
                    .AppendDescription($"I couldn't find property named `{name}`.");
            }
            await this.ReplyAsync("", false, message.ToEmbed());
        }
    }
}
