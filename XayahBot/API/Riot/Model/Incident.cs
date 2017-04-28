using System.Collections.Generic;

namespace XayahBot.API.Riot.Model
{
    public class Incident
    {
        public bool Active { get; set; }
        public string Created_At { get; set; }
        public long Id { get; set; }
        public List<Message> Updates { get; set; }
    }
}
