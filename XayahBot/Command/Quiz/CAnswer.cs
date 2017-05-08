#pragma warning disable 4014

using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Command.Precondition;

namespace XayahBot.Command.Quiz
{
    [Category(CategoryType.QUIZ)]
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
