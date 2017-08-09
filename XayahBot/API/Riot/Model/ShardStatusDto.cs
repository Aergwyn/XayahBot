using System.Collections.Generic;
using Newtonsoft.Json;

namespace XayahBot.API.Riot.Model
{
    public class ShardStatusDto
    {
        //public string HostName { get; set; }
        //public List<string> Locales { get; set; }
        [JsonProperty("Name")]
        public string Region { get; set; }
        //public string Region_Tag { get; set; }
        public List<ServiceDto> Services { get; set; }
        //public string Slug { get; set; }

        public override string ToString()
        {
            return this.Region;
        }
    }
}
