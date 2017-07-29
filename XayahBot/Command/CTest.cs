using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Utility;

namespace XayahBot.Command
{
    public class CTest : ModuleBase
    {
        [Command("test")]
        public async Task Test(ulong messageId = 0)
        {
            if (messageId.Equals(0))
            {
                IMessage message = await this.ReplyAsync("This is a test") as IMessage;

                await this.ReplyAsync($"id: {message.Id}");
            }
            else
            {
                try
                {
                    IMessage testing = await this.Context.Channel.GetMessageAsync(messageId);
                    if (testing == null)
                    {
                        await this.ReplyAsync("null");
                    }
                    else
                    {
                        await this.ReplyAsync("not null");
                    }
                }
                catch (Exception ex)
                {
                    await Logger.Error("halp?", ex);
                    await this.ReplyAsync("halp?");
                }
            }
        }
    }
}
