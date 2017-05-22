using Newtonsoft.Json;

namespace XayahBot.API.Riot.Model
{
    public class ChampionMasteryDto
    {
        public long ChampionId { get; set; }
        //public int ChampionLevel { get; set; }
        public int ChampionPoints { get; set; }
        //public long ChampionPointsSinceLastLevel { get; set; }
        //public long ChampionPointsUntilNextLevel { get; set; }
        //public bool ChestGranted { get; set; }
        //public long LastPlayTime { get; set; }
        [JsonProperty("PlayerId")]
        public long SummonerId { get; set; }
    }
}
