using System.Collections.Generic;

namespace XayahBot.API.Riot.Model
{
    public class ShardStatus
    {
        public string Name { get; set; }
        public string Region_Tag { get; set; }
        public string HostName { get; set; }
        public List<Service> Services { get; set; }
        public string Slug { get; set; }
        public List<string> Locales { get; set; }
    }
}
