using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Service;

namespace XayahBot.Command
{
    public class CQuiz : ModuleBase
    {
#pragma warning disable 4014 // Intentional
        [Command("quiz")]
        [RequireContext(ContextType.Guild)]
        public Task Quiz([Remainder] string trash = "")
        {
            QuizService.AskQuestionAsync(this.Context);
            return Task.CompletedTask;
        }
#pragma warning restore 4014

#pragma warning disable 4014 // Intentional
        [Command("answer")]
        [RequireContext(ContextType.Guild)]
        public Task Answer([Remainder] string answer = "")
        {
            QuizService.AnswerQuestionAsync(this.Context, answer);
            return Task.CompletedTask;
        }
#pragma warning restore 4014
    }
}
