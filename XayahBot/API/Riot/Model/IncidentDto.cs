using System.Collections.Generic;
using Newtonsoft.Json;

namespace XayahBot.API.Riot.Model
{
    public class IncidentDto
    {
        public bool Active { get; set; }

        //[JsonProperty("Created_At")]
        //public string CreationTime { get; set; }

        //public long Id { get; set; }

        public List<UpdateDto> Updates { get; set; }
    }
}
