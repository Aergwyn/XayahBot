using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using XayahBot.Command.Attribute;
using XayahBot.Utility;

namespace XayahBot.Command
{
    public class CGame : ModuleBase
    {
        public DiscordSocketClient Client { get; set; }

        //

        public CGame(DiscordSocketClient client)
        {
            this.Client = client;
        }

        //

        private readonly string _logRequest = "\"{0}\" requested \"status\".";
        private readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";
        private readonly string _logChanged = "Status changed to \"{0}\".";

        //

#pragma warning disable 4014 // Intentional
        [Command("game")]
        [RequireMod]
        [Summary("Updates the current game.")]
        public async Task Game([Remainder] string text = "")
        {
            Logger.Log(LogSeverity.Debug, nameof(CProperty), string.Format(this._logRequest, this.Context.User));
            IMessageChannel channel = null;
            if (this.Context.IsPrivate)
            {
                channel = this.Context.Channel;
            }
            else
            {
                channel = await this.Context.Message.Author.CreateDMChannelAsync();
            }
            if (channel == null)
            {
                Logger.Log(LogSeverity.Error, nameof(CProperty), string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string game = string.IsNullOrWhiteSpace(text) ? null : text.Trim();
            Property.GameActive.Value = game;
            this.Client.SetGameAsync(game);
            Logger.Log(LogSeverity.Debug, nameof(CProperty), string.Format(this._logChanged, game));
        }
#pragma warning restore 4014
    }
}
