#pragma warning disable 4014

using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Command.Incidents;
using XayahBot.Command.Remind;
using XayahBot.Database.DAO;
using XayahBot.Error;

namespace XayahBot.Utility
{
    public class DiscordEventHandler
    {
        private readonly IDependencyMap _dependencyMap;
        private readonly IgnoreListDAO _ignoreListDao = new IgnoreListDAO();
        private readonly IncidentSubscriberDAO _incidentSubscriberDao = new IncidentSubscriberDAO();

        public DiscordEventHandler(IDependencyMap dependencyMap)
        {
            this._dependencyMap = dependencyMap;
        }

        public async Task HandleReady()
        {
            await this.SetGameAsync();
            await this.StartBackgroundThreadsAsync();
        }

        private Task SetGameAsync()
        {
            DiscordSocketClient client = this._dependencyMap.Get<DiscordSocketClient>();
            string game = string.IsNullOrWhiteSpace(Property.GameActive.Value) ? null : Property.GameActive.Value;
            client.SetGameAsync(game);
            return Task.CompletedTask;
        }

        private async Task StartBackgroundThreadsAsync()
        {
            RemindService remindService = this._dependencyMap.Get<RemindService>();
            IncidentService incidentService = this._dependencyMap.Get<IncidentService>();
            await remindService.StartAsync();
            await incidentService.StartAsync();
        }

        public async Task HandleChannelDestroyed(SocketChannel deletedChannel)
        {
            if (this._ignoreListDao.HasSubject(deletedChannel.Id))
            {
                await this._ignoreListDao.RemoveBySubjectIdAsync(deletedChannel.Id);
            }
            if (this._incidentSubscriberDao.HasSubscriber(deletedChannel.Id))
            {
                await this._incidentSubscriberDao.RemoveByChannelIdAsync(deletedChannel.Id);
            }
        }

        public async Task HandleLeftGuild(SocketGuild leftGuild)
        {
            await this._ignoreListDao.RemoveByGuildIdAsync(leftGuild.Id);
            await this._incidentSubscriberDao.RemoveByGuildIdAsync(leftGuild.Id);
        }
    }
}
