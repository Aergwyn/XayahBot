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
        public Task Quiz(string option = "", [Remainder] string trash = "")
        {
            // TODO add modes
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
