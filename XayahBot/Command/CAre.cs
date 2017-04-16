using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Utility;

namespace XayahBot.Command
{
    public class CAre : ModuleBase
    {
        private static readonly List<string> _responses = new List<string>()
        {
            // Positive
            "Yes, totally!",
            "I kinda have to agree there.",
            "Yup.",
            "This is a bit weird but I'd say yes.",
            "Rakan agrees. So I can't say no.",
            "Alright!",
            // Neutral
            "It's not impossible? *shrugs*",
            "Maybe...",
            "*looks confused*",
            "Yesnoidonotwanttoanswerthis! *gasps for air*",
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
        private static readonly List<string> _noQuestion = new List<string>()
        {
            "Did you not learn how to correctly phrase a question?",
            "I could sell you a beautiful set of question marks.",
            "Was that for me?",
            "I nearly thought you were asking me something.",
        };
        private static readonly List<string> _noSentence = new List<string>()
        {
            "I hate lazy people like you that can't finish a sentence properly.",
            "I think you missed the point. Like... literally.",
            "Aaaand another day with lazy humans.",
            "How am I supposed to work with... whatever this is?",
        };

        //
        
        [Command("are"), Alias("is", "am")]
        public Task Are([Remainder] string message = "")
        {
            string response = string.Empty;
            message = message.Trim();
            if (message.EndsWith("?"))
            {
                response = _responses.ElementAt(RNG.Next(_responses.Count()) - 1);
            }
            else if (message.EndsWith(".") || message.EndsWith("!"))
            {
                response = _noQuestion.ElementAt(RNG.Next(_noQuestion.Count()) - 1);
            }
            else
            {
                response = _noSentence.ElementAt(RNG.Next(_noSentence.Count()) - 1);
            }
            ReplyAsync(response);
            return Task.CompletedTask;
        }
    }
}
