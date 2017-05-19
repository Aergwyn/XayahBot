using System.Threading.Tasks;
using XayahBot.API.Riot.Model;

namespace XayahBot.API.Riot
{
    public class StatusApi : RiotApi
    {
        public StatusApi(Region region) : base(region)
        {
        }

        protected override string GetService()
        {
            return "status";
        }

        protected override string GetVersion()
        {
            return "v3";
        }

        public async Task<ShardStatusDto> GetStatusAsync()
        {
            return await this.GetAsync<ShardStatusDto>(new ApiRequest("shard-data"));
        }
    }
}
