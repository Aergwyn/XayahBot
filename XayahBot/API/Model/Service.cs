using System.Collections.Generic;

namespace XayahBot.API.Model
{
    public class Service
    {
        public string Status { get; set; }
        public List<Incident> Incidents { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}
