using System;
using System.Threading.Tasks;
using XayahBot.API.Riot.Model;

namespace XayahBot.API.Riot
{
    public class RiotStaticDataApi : RiotCachedApi
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

        protected override DateTime GetDataExpirationTime()
        {
            return DateTime.UtcNow.AddHours(24);
        }

        public async Task<ChampionListDto> GetChampionsAsync()
        {
            return await this.GetAsync<ChampionListDto>(new ApiRequest("champions", "dataById=true"));
        }

        public async Task<ChampionDto> GetChampionAsync(int id)
        {
            return await this.GetAsync<ChampionDto>(new ApiRequest($"champions/{id}",
            "champData=partype", "champData=passive", "champData=skins", "champData=spells", "champData=stats", "champData=tags"));
        }
    }
}
