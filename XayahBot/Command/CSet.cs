using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using XayahBot.Command.Attribute;
using XayahBot.Utility;

namespace XayahBot.Command
{
    [Group("set")]
    public class CSet : ModuleBase
    {
        public DiscordSocketClient Client { get; set; }

        //

        public CSet(DiscordSocketClient client)
        {
            this.Client = client;
        }

        //

        private readonly string _logRequest = "\"{0}\" requested \"{1}\".";
        private readonly string _logChanged = "\"{0}\" changed to \"{1}\".";

        private readonly string _notFound = "Could not find property with name `{0}`!";
        private readonly string _changed = $"Property changed...{Environment.NewLine}```Old: {{0}}:\"{{1}}\"{Environment.NewLine}New: {{0}}:\"{{2}}\"```";

        private readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";

        //

        [Command("property"), Alias("p")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        [Summary("Updates a specific property.")]
        public Task Property(string name, [Remainder]string value = "")
        {
            Logger.Log(LogSeverity.Warning, nameof(CSet), string.Format(this._logRequest, this.Context.User, "set property"));

            Property property = Utility.Property.GetByName(name);
            if (property != null)
            {
                string oldValue = property.Value;
                property.Value = value.Trim();
                ReplyAsync(string.Format(this._changed, property.Name, oldValue, property.Value));
                Logger.Log(LogSeverity.Warning, nameof(CSet), string.Format(this._logChanged, property.Name, property.Value));
            }
            else
            {
                ReplyAsync(string.Format(this._notFound, name));
            }
            return Task.CompletedTask;
        }

        //

#pragma warning disable 4014 // Intentional
        [Command("game")]
        [RequireMod]
        [Summary("Updates the current game-status.")]
        public async Task Game([Remainder] string status = "")
        {
            Logger.Log(LogSeverity.Debug, nameof(CSet), string.Format(this._logRequest, this.Context.User, "set game"));
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
                Logger.Log(LogSeverity.Error, nameof(CSet), string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string game = string.IsNullOrWhiteSpace(status) ? null : status.Trim();
            Utility.Property.GameActive.Value = game;
            this.Client.SetGameAsync(game);
            Logger.Log(LogSeverity.Debug, nameof(CSet), string.Format(this._logChanged, Utility.Property.GameActive.Name, game));
        }
#pragma warning restore 4014
    }
}
