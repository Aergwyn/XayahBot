using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Service;
using XayahBot.Utility;

namespace XayahBot.Command
{
    public class CData : ModuleBase
    {
        private static readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";

        //

#pragma warning disable 4014 // Intentional
        [Command("data")]
        public async Task Data(/*string option = "", */[Remainder] string arg = "")
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
                Logger.Log(LogSeverity.Error, nameof(CData), string.Format(_logNoReplyChannel, this.Context.User));
                return;
            }
            // TODO add options
            InfoService.GetChampionData(channel, arg);
        }
#pragma warning restore 4014
    }
}
