using System.Collections.Generic;

namespace XayahBot.API.Riot.Model
{
    public class ChampionDto
    {
        //public List<string> AllyTips { get; set; }
        //public string Blurb { get; set; } // Mini-Lore or something...
        //public List<string> EnemyTips { get; set; }
        public int Id { get; set; }
        //public ImageDto Image { get;set;}
        //public InfoDto Info { get; set; } // Attack, Defense, Difficulty, ...
        //public string Key { get; set; }
        //public string Lore { get; set; }
        public string Name { get; set; }
        public string ParType { get; set; } // Resource
        public PassiveDto Passive { get; set; }
        //public List<RecommendedDto> Recommended { get; set; }
        public List<SkinDto> Skins { get; set; }
        public StatsDto Stats { get; set; }
        public List<ChampionSpellDto> Spells { get; set; }
        public List<string> Tags { get; set; } // aka Mage, Marksman, ...
        public string Title { get; set; }

        //

        public ChampionDto()
        {
            // for JSON
        }

        public ChampionDto(ChampionDto champion)
        {
            this.Id = champion.Id;
            this.Name = champion.Name;
            this.ParType = champion.ParType;
            this.Passive = champion.Passive;
            this.Skins = new List<SkinDto>(champion.Skins);
            this.Stats = champion.Stats;
            this.Spells = new List<ChampionSpellDto>(champion.Spells);
            this.Tags = champion.Tags;
            this.Title = champion.Title;
        }

        //

        public ChampionDto Copy()
        {
            return new ChampionDto(this);
        }
    }
}
