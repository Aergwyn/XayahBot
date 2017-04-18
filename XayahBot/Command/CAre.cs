using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Utility;
using XayahBot.Command.Attribute;

namespace XayahBot.Command
{
    public class CAre : ModuleBase
    {
        private readonly List<string> _responses = new List<string>()
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
        private readonly List<string> _noQuestion = new List<string>()
        {
            "Did you not learn how to correctly phrase a question?",
            "I could sell you a beautiful set of question marks.",
            "Was that for me?",
            "I nearly thought you were asking me something.",
        };
        private readonly List<string> _noSentence = new List<string>()
        {
            "I hate lazy people like you that can't finish a sentence properly.",
            "I think you missed the point. Like... literally.",
            "Aaaand another day with lazy humans.",
            "How am I supposed to work with... whatever this is?",
        };

        //
        
        [Command("are"), Alias("is", "am")]
        [CheckIgnoredUser]
        [CheckIgnoredChannel]
        [Summary("Triggers an 8ball-esque response.")]
        public Task Are([Remainder] string text = "")
        {
            string response = string.Empty;
            text = text.Trim();
            if (text.EndsWith("?"))
            {
                response = this._responses.ElementAt(RNG.Next(this._responses.Count) - 1);
            }
            else if (text.EndsWith(".") || text.EndsWith("!"))
            {
                response = this._noQuestion.ElementAt(RNG.Next(this._noQuestion.Count) - 1);
            }
            else
            {
                response = this._noSentence.ElementAt(RNG.Next(this._noSentence.Count) - 1);
            }
            ReplyAsync(response);
            return Task.CompletedTask;
        }
    }
}
