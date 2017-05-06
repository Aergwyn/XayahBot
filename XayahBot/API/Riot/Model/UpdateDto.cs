using Newtonsoft.Json;

namespace XayahBot.API.Riot.Model
{
    public class UpdateDto
    {
        public string Author { get; set; }
        public string Content { get; set; }
        //[JsonProperty("Created_At")]
        //public string CreationTime { get; set; }
        public string Id { get; set; }
        public string Severity { get; set; }
        //public List<Translation> Translations { get; set; }
        [JsonProperty("Updated_At")]
        public string UpdateTime { get; set; }
    }
}
