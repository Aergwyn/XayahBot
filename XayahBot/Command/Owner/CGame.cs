using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Command.System;
using XayahBot.Utility;

namespace XayahBot.Command.Owner
{
    public class CGame : LoggedModuleBase
    {
        [Command("game")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        [Summary("Updates the current game.")]
        public Task Game([Remainder] string text = "")
        {
            DiscordSocketClient client = this.Context.Client as DiscordSocketClient;
            string game = null;
            if (!string.IsNullOrWhiteSpace(text))
            {
                game = text.Trim();
            }
            Property.GameActive.Value = game;
            client.SetGameAsync(game);
            return Task.CompletedTask;
        }
    }
}
