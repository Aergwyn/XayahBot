using Discord.Commands;
using System.Threading.Tasks;
using XayahBot.Command.Attribute;
using XayahBot.Service;

namespace XayahBot.Command
{
    public class CAnswer : ModuleBase
    {
#pragma warning disable 4014 // Intentional
        [Command("answer")]
        [RequireContext(ContextType.Guild)]
        [CheckIgnoredUser]
        [CheckIgnoredChannel]
        [Summary("Answers a previously opened question.")]
        public Task Answer([Remainder] string text)
        {
            QuizService.AnswerQuestionAsync(this.Context, text);
            return Task.CompletedTask;
        }
#pragma warning restore 4014
    }
}
