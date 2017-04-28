using System.Collections.Generic;

namespace XayahBot.API.Riot.Model
{
    public class Message
    {
        public string Severity { get; set; }
        public string Author { get; set; }
        public string Created_At { get; set; }
        public List<Translation> Translations { get; set; }
    }
}
