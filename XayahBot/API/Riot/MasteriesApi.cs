using System.Threading.Tasks;
using XayahBot.API.Riot.Model;

namespace XayahBot.API.Riot
{
    public class MasteriesApi : RiotApi
    {
        public MasteriesApi(Region region) : base(region)
        {
        }

        protected override string GetService()
        {
            return "masteries";
        }

        protected override string GetVersion()
        {
            return "v3";
        }

        protected override string GetBaseUrl()
        {
            return $"https://{this._region.Platform}.api.riotgames.com/lol/platform/{this.GetVersion()}/{this.GetService()}";
        }

        public async Task<MasteryPagesDto> GetMasteriesBySummonerIdAsync(long summonerId)
        {
            return await this.GetAsync<MasteryPagesDto>(new ApiRequest($"by-summoner/{summonerId}"));
        }
    }
}
