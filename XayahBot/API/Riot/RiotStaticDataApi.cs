using System.Threading.Tasks;
using XayahBot.API.Riot.Model;

namespace XayahBot.API.Riot
{
    public class RiotStaticDataApi : RiotApi
    {
        public RiotStaticDataApi(Region region = null) : base(region)
        {
        }

        protected override string GetService()
        {
            return "static-data";
        }

        protected override string GetVersion()
        {
            return "v3";
        }

        //

        public async Task<ChampionListDto> GetChampionsAsync()
        {
            return await this.GetAsync<ChampionListDto>(new ApiRequest("champions", "dataById=true"));
        }

        public async Task<ChampionDto> GetChampionAsync(int id)
        {
            return await this.GetAsync<ChampionDto>(new ApiRequest($"champions/{id}", "champData=partype,passive,skins,spells,stats,tags"));
        }
    }
}
