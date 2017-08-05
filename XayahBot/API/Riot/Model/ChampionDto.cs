using System.Collections.Generic;
using Newtonsoft.Json;

namespace XayahBot.API.Riot.Model
{
    public class ChampionDto
    {
        //public List<string> AllyTips { get; set; }
        //public string Blurb { get; set; } // Mini-Lore or something...
        //public List<string> EnemyTips { get; set; }
        public int Id { get; set; }
        //public ImageDto Image { get; set; }
        //public InfoDto Info { get; set; } // Attack, Defense, Difficulty, ...
        //public string Key { get; set; }
        //public string Lore { get; set; }
        public string Name { get; set; }
        [JsonProperty("ParType")]
        public string Resource { get; set; }
        public PassiveDto Passive { get; set; }
        //public List<RecommendedDto> Recommended { get; set; }
        public List<SkinDto> Skins { get; set; }
        public StatsDto Stats { get; set; }
        public List<ChampionSpellDto> Spells { get; set; }
        public List<string> Tags { get; set; } // aka Mage, Marksman, ...
        public string Title { get; set; }
    }
}
