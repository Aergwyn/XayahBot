using System;
using System.Threading.Tasks;
using XayahBot.API.Riot.Model;

namespace XayahBot.API.Riot
{
    public class ChampionMasteryApi : RiotCachedApi
    {
        public ChampionMasteryApi(Region region) : base(region)
        {
        }

        protected override string GetService()
        {
            return "champion-mastery";
        }

        protected override string GetVersion()
        {
            return "v3";
        }

        protected override DateTime GetDataExpirationTime()
        {
            return DateTime.UtcNow.AddHours(1);
        }

        public async Task<ChampionMasteryDto> GetChampionMastery(long summonerId, int championId)
        {
            return await this.GetAsync<ChampionMasteryDto>(new ApiRequest($"by-summoner/{summonerId}/by-champion/{championId}"));
        }
    }
}
