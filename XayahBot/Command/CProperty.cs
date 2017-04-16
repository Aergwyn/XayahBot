using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Utility;
using XayahBot.Command.Attribute;

namespace XayahBot.Command
{
    public class CProperty : ModuleBase
    {
        public DiscordSocketClient Client { get; set; }

        //

        public CProperty(DiscordSocketClient client)
        {
            this.Client = client;
        }

        //

        private static readonly string _logRequest = "\"{0}\" requested \"{1}\".";
        private static readonly string _logChanged = "\"{0}\" changed to \"{1}\".";
        private static readonly string _noPermission = "You do NOT have permission to do that! Ask {0} and maybe you're lucky.";

        private static readonly string _notFound = "Could not find property with name `{0}`!";
        private static readonly string _changed = $"Property changed...{Environment.NewLine}```Old: {{0}}:\"{{1}}\"{Environment.NewLine}New: {{0}}:\"{{2}}\"```";
        private static readonly string _currentValue = "```{0}:\"{1}\"```";

        private static readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";

        //

        [Command("get property"), Alias("gp")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        [Summary("Lists all or the specified property.")]
        public Task GetProperty(string name = "")
        {
            Logger.Log(LogSeverity.Debug, nameof(CProperty), string.Format(_logRequest, this.Context.User, "get property"));

            if (!string.IsNullOrWhiteSpace(name))
            {
                Property property = Property.GetByName(name);
                if (property != null)
                {
                    ReplyAsync(string.Format(_currentValue, property.Name, property.Value));
                }
                else
                {
                    ReplyAsync(string.Format(_notFound, name));
                }
            }
            else
            {
                string values = $"__List of properties__{Environment.NewLine}```";
                List<Property> displayList = Property.Values.Where(x => x.Updatable).ToList();
                for (int i = 0; i < displayList.Count; i++)
                {
                    if (i > 0)
                    {
                        values += Environment.NewLine;
                    }
                    values += $"{(displayList[i].Name + ":").PadRight(Property.Values.Where(x => x.Updatable).OrderByDescending(x => x.Name.Length).First().Name.Length + 1, ' ')}\"{displayList[i].Value}\"";
                }
                values += "```";
                ReplyAsync(values);
            }
            return Task.CompletedTask;
        }

        [Command("set property"), Alias("sp")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        [Summary("Updates specified property.")]
        public Task SetProperty(string name, [Remainder]string value = "")
        {
            Logger.Log(LogSeverity.Warning, nameof(CProperty), string.Format(_logRequest, this.Context.User, "set property"));

            Property property = Property.GetByName(name);
            if (property != null)
            {
                string oldValue = property.Value;
                property.Value = value.Trim();
                ReplyAsync(string.Format(_changed, property.Name, oldValue, property.Value));
                Logger.Log(LogSeverity.Warning, nameof(CProperty), string.Format(_logChanged, property.Name, property.Value));
            }
            else
            {
                ReplyAsync(string.Format(_notFound, name));
            }
            return Task.CompletedTask;
        }

        //

#pragma warning disable 4014 // Intentional
        [Command("set game")]
        [RequireMod]
        [Summary("Updates the current game-status.")]
        public async Task SetGame([Remainder] string status = "")
        {
            Logger.Log(LogSeverity.Debug, nameof(CProperty), string.Format(_logRequest, this.Context.User, "set game"));
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
                Logger.Log(LogSeverity.Error, nameof(CProperty), string.Format(_logNoReplyChannel, this.Context.User));
                return;
            }
            string game = string.IsNullOrWhiteSpace(status) ? null : status.Trim();
            Property.GameActive.Value = game;
            this.Client.SetGameAsync(game);
            Logger.Log(LogSeverity.Debug, nameof(CProperty), string.Format(_logChanged, Property.GameActive.Name, game));
        }
#pragma warning restore 4014
    }
}
