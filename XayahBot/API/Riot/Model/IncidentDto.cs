using System.Collections.Generic;

namespace XayahBot.API.Riot.Model
{
    public class IncidentDto
    {
        public bool Active { get; set; }
        //[JsonProperty("Created_At")]
        //public string CreationTime { get; set; }
        public long Id { get; set; }
        public List<UpdateDto> Updates { get; set; }

        public override string ToString()
        {
            return this.Id.ToString();
        }
    }
}
