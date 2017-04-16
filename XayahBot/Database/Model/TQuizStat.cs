using System;

namespace XayahBot.Database.Model
{
    public class TQuizStat
    {
        public int Id { get; set; }
        public ulong Guild { get; set; }
        public string User { get; set; }
        public int Answers { get; set; }
    }
}
