using System.Collections.Generic;

namespace XayahBot.API.Riot.Model
{
    public class ChampionListDto
    {
        public Dictionary<string, ChampionDto> Data { get; set; }
        //public string Format { get; set; }
        //public Dictionary<string, string> Keys { get; set; }
        //public string Type { get; set; }
        //public string Version { get; set; }

        //

        public ChampionListDto()
        {
            // for JSON
        }

        public ChampionListDto(Dictionary<string, ChampionDto> data)
        {
            this.Data = data;
        }

        //

        public ChampionListDto Copy()
        {
            return new ChampionListDto(new Dictionary<string, ChampionDto>(this.Data));
        }
    }
}
