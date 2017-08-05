using System;
using System.Threading.Tasks;
using XayahBot.API.Riot.Model;

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

        protected override DateTime GetDataExpirationTime()
        {
            DateTime now = DateTime.UtcNow;
            return new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(1);
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
