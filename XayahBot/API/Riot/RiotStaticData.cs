using System.Threading.Tasks;
using XayahBot.API.Riot.Model;
using XayahBot.Utility;

namespace XayahBot.API.Riot
{
    public class RiotStaticData : RiotApi
    {
        public RiotStaticData(Region region) : base(region)
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

        public async Task<ChampionListDto> GetChampionsAsync()
        {
            return await this.GetAsync<ChampionListDto>(new ApiRequest("champions", "dataById=true"), TimeUtil.InDays(1));
        }

        public async Task<ChampionDto> GetChampionAsync(int id)
        {
            return await this.GetAsync<ChampionDto>(new ApiRequest($"champions/{id}",
                "champData=partype", "champData=passive", "champData=skins", "champData=spells", "champData=stats", "champData=tags"),
                TimeUtil.InDays(2));
        }

        public async Task<ItemListDto> GetItemsAsync()
        {
            return await this.GetAsync<ItemListDto>(new ApiRequest("items", "tags=gold", "tags=from", "tags=into", "tags=sanitizedDescription"), TimeUtil.InDays(1));
        }
    }
}
