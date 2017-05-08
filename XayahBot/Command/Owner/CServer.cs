using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Command.Precondition;
using XayahBot.Utility;
using XayahBot.Utility.Messages;

namespace XayahBot.Command.Owner
{
    [Category(CategoryType.OWNER)]
    public class CServer : ModuleBase
    {
        [Command("server")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        [Summary("Displays all connected server.")]
        public Task Server()
        {
            DiscordSocketClient client = this.Context.Client as DiscordSocketClient;
            DiscordFormatEmbed message = new DiscordFormatEmbed()
                .AppendDescription($"Here is a list of all server I'm currently on.{Environment.NewLine}{Environment.NewLine}")
                .AppendDescription(ListUtil.BuildEnumeration(client.Guilds));
            this.ReplyAsync("", false, message.ToEmbed());
            return Task.CompletedTask;
        }
    }
}
