#pragma warning disable 4014

using Discord.Commands;
using System.Threading.Tasks;
using XayahBot.Command.Attribute;
using XayahBot.Service;

namespace XayahBot.Command.Quiz
{
    public class CAnswer : ModuleBase
    {
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
    }
}
