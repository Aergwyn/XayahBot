using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Error;
using XayahBot.Extension;
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
            Task.Run(() => this.ProcessGet());
            return Task.CompletedTask;
        }

        private async Task ProcessGet()
        {
            try
            {
                string text = string.Empty;
                int maxWidth = Property.UpdatableValues().OrderByDescending(x => x.Name.Length).First().Name.Length;
                IEnumerable<Property> properties = Property.UpdatableValues();
                for (int i = 0; i < properties.Count(); i++)
                {
                    if (i > 0)
                    {
                        text += Environment.NewLine;
                    }
                    Property property = properties.ElementAt(i);
                    text += property.Name.PadRight(maxWidth) + ":" + property.Value;
                }
                FormattedEmbedBuilder message = new FormattedEmbedBuilder()
                    .AppendTitle($"{XayahReaction.Option} Property list")
                    .AppendDescription(text, AppendOption.Codeblock);
                await this.ReplyAsync(message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        [Command("set")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        public Task Set(string name, [Remainder]string value = "")
        {
            Task.Run(() => this.ProcessSet(name, value));
            return Task.CompletedTask;
        }

        private async Task ProcessSet(string name, string value)
        {
            try
            {
                value = value.Trim();
                FormattedEmbedBuilder message = new FormattedEmbedBuilder();
                try
                {
                    Property property = Property.GetUpdatableByName(name);
                    string oldValue = property.Value;
                    property.Value = value;
                    message
                        .AppendTitle($"{XayahReaction.Success} Done")
                        .AppendDescription($"I updated `{name}` for you.")
                        .AppendDescription($"Old:{oldValue}{Environment.NewLine}New:{value}", AppendOption.Codeblock);
                }
                catch (NotExistingException)
                {
                    message
                        .AppendTitle($"{XayahReaction.Error} This didn't work")
                        .AppendDescription($"I couldn't find a property named `{name}`.");
                }
                await this.ReplyAsync(message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
