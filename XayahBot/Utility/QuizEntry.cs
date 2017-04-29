using System;
using System.Linq;

namespace XayahBot.Utility
{
    public class QuizEntry
    {
        public string Question { get; private set; }
        public string[] Answer { get; private set; }
        public int MatchPercentage { get; private set; }

        public DateTime TimeAsked { get; private set; }
        public int TimesFailed { get; set; }

        public QuizEntry(string question, params string[] answer) : this(question, 100, answer)
        {
        }

        public QuizEntry(string question, int matchPercentage, params string[] answer)
        {
            this.Question = question;
            this.Answer = answer;
            this.MatchPercentage = matchPercentage;
            this.TimeAsked = DateTime.UtcNow;

            for (int i = 0; i < this.Answer.Count(); i++)
            {
                this.Answer[i] = this.Answer[i].Trim();
            }
        }

        //

        public string GetAllAnswers()
        {
            string text = string.Empty;
            for (int i = 0; i < this.Answer.Count(); i++)
            {
                if (i > 0)
                {
                    if (i == this.Answer.Count() - 1)
                    {
                        text += " or ";
                    }
                    else
                    {
                        text += ", ";
                    }
                }
                text += $"`{this.Answer.ElementAt(i)}`";
            }
            return text;
        }
    }
}
