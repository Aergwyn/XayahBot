#pragma warning disable 4014

using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Utility;

namespace XayahBot.Command.Data
{
    [Group("data")]
    public class CData : ModuleBase
    {
        private readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";

        //

        [Command("champ"), Alias("c")]
        [Summary("Displays data of a specific champion. Name does not have to be exact.")]
        public async Task Champ([Remainder] string name)
        {
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
                Logger.Error(string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            InfoService.GetChampionData(channel, name);
        }
    }
}
