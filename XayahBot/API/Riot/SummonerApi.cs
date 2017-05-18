using System;
using System.Threading.Tasks;
using XayahBot.API.Riot.Model;

namespace XayahBot.API.Riot
{
    public class SummonerApi : RiotCachedApi
    {
        public SummonerApi(Region region) : base(region)
        {
        }

        protected override string GetService()
        {
            return "summoner";
        }

        protected override string GetVersion()
        {
            return "v3";
        }

        protected override DateTime GetDataExpirationTime()
        {
            return DateTime.UtcNow.AddHours(24);
        }

        public async Task<SummonerDto> GetSummonerByNameAsync(string summonerName)
        {
            return await this.GetAsync<SummonerDto>(new ApiRequest($"summoners/by-name/{summonerName}"));
        }
    }
}
