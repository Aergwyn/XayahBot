using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Utility;

namespace XayahBot.Command.Owner
{
    public class CGame : ModuleBase
    {
        [Command("game")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        public Task Game([Remainder] string text = "")
        {
            Task.Run(() => this.SetGame(text));
            return Task.CompletedTask;
        }

        private async Task SetGame(string text)
        {
            DiscordSocketClient client = this.Context.Client as DiscordSocketClient;
            string game = null;
            if (!string.IsNullOrWhiteSpace(text))
            {
                game = text.Trim();
            }
            Property.GameActive.Value = game;
            await client.SetGameAsync(game);
        }
    }
}
