#pragma warning disable 4014

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using XayahBot.Utility;

namespace XayahBot.Command
{
    public class CGame : ModuleBase
    {
        private readonly string _logRequest = "\"{0}\" requested \"status\".";
        private readonly string _logChanged = "Status changed to \"{0}\".";

        //

        private readonly DiscordSocketClient _client;

        public CGame(DiscordSocketClient client)
        {
            this._client = client;
        }

        //

        [Command("game")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        [Summary("Updates the current game.")]
        public Task Game([Remainder] string text = "")
        {
            Logger.Debug(string.Format(this._logRequest, this.Context.User));
            string game = string.IsNullOrWhiteSpace(text) ? null : text.Trim();
            Property.GameActive.Value = game;
            this._client.SetGameAsync(game);
            Logger.Debug(string.Format(this._logChanged, game));
            return Task.CompletedTask;
        }
    }
}
