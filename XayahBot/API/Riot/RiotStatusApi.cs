using System.Threading.Tasks;
using XayahBot.API.Riot.Model;

namespace XayahBot.API
{
    public class RiotStatusApi : RiotApi
    {
        public RiotStatusApi(Region region) : base(region)
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

        //

        public async Task<ShardStatus> GetStatusAsync()
        {
            return await this.GetAsync<ShardStatus>(new ApiRequest("shard-data"));
        }
    }
}
