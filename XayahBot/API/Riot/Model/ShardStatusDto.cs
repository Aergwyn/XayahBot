using System.Collections.Generic;
using Newtonsoft.Json;

namespace XayahBot.API.Riot.Model
{
    public class ShardStatusDto
    {
        //public string HostName { get; set; }
        //public List<string> Locales { get; set; }
        public string Name { get; set; }

        //[JsonProperty("Region_Tag")]
        //public string Region { get; set; }

        public List<ServiceDto> Services { get; set; }
        //public string Slug { get; set; }
    }
}
