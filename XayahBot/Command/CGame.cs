#pragma warning disable 4014

using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Command.System;
using XayahBot.Utility;

namespace XayahBot.Command
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
            string game = string.IsNullOrWhiteSpace(text) ? null : text.Trim();
            Property.GameActive.Value = game;
            client.SetGameAsync(game);
            return Task.CompletedTask;
        }
    }
}
