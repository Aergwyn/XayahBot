using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Utility;

namespace XayahBot.Command.Misc
{
    public class CAre : ModuleBase
    {
        private readonly List<string> _responseList = new List<string>
        {
            // Positive
            "Yes, totally!",
            "I kinda have to agree there.",
            "Yup.",
            "This is a bit weird but I'd say yes.",
            "Alright!",
            "Rakan agrees. So I can't say no.",
            // Neutral
            "Maybe...",
            "It's not impossible? *shrugs*",
            "Yesnoidonotwanttoanswerthis! *gasps for air*",
            "*looks confused*",
            "Uhm...",
            "I'm a little busy at the moment.",
            // Negative
            "I don't really want to talk right now.",
            "I do my thing and you do yours. But go over there first.",
            "Rakan disagrees, so do I.",
            "Behold, I am depleted of cares.",
            "Just... no.",
            "Oh, hell NO.",
        };
        private readonly List<string> _noQuestionList = new List<string>
        {
            "Didn't you learn how to correctly phrase a question?",
            "I could sell you a beautiful set of question marks.",
            "Was that for me?",
            "I nearly thought you were asking me something.",
        };
        private readonly List<string> _noSentenceList = new List<string>
        {
            "I hate lazy people like you that can't finish a sentence properly.",
            "I think you missed the point. Like... literally.",
            "Aaaand another day with lazy humans.",
            "How am I supposed to work with... whatever this is?",
        };

        [Command("are"), Alias("is", "am")]
        public Task Are([Remainder] string text = "")
        {
            Task.Run(() => this.ProcessAre(text));
            return Task.CompletedTask;
        }

        private async Task ProcessAre(string text)
        {
            text = text.Trim();
            string response = string.Empty;
            if (this.IsQuestion(text))
            {
                response = RNG.FromList(this._responseList);
            }
            else if (this.IsSentence(text))
            {
                response = RNG.FromList(this._noQuestionList);
            }
            else
            {
                response = RNG.FromList(this._noSentenceList);
            }
            await this.ReplyAsync(response);
        }

        private bool IsQuestion(string text)
        {
            if (text.EndsWith("?"))
            {
                return true;
            }
            return false;
        }

        private bool IsSentence(string text)
        {
            if (text.EndsWith(".") || text.EndsWith("!"))
            {
                return true;
            }
            return false;
        }
    }
}
