using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Misc
{
    public class CHelp : ModuleBase
    {
        [Command("help")]
        public async Task Help([Remainder] string trash = "")
        {
            IMessageChannel channel = await ChannelProvider.GetDMChannelAsync(this.Context);
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .AppendDescription($"I'm currently undergoing a rework so there is nothing I can help you with.")
                .AppendDescription(Environment.NewLine);
            await channel.SendMessageAsync("", false, message.ToEmbed());
        }
    }
}
