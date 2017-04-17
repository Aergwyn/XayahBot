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
        [Summary("Asks a random question about a champion.")]
        public Task Quiz(/*string mode = ""*/)
        {
            QuizService.AskQuestionAsync(this.Context);
            return Task.CompletedTask;
        }
#pragma warning restore 4014
    }
}
