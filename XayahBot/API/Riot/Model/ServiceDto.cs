using System.Collections.Generic;

namespace XayahBot.API.Riot.Model
{
    public class ServiceDto
    {
        public List<IncidentDto> Incidents { get; set; }
        public string Name { get; set; }
        //public string Slug { get; set; }
        public string Status { get; set; }
    }
}
