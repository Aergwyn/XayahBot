using System.Threading.Tasks;
using XayahBot.API.Model;
using XayahBot.Utility;

namespace XayahBot.API.Controller
{
    public class LolStatusV3Controller : AbstractController<ShardStatus>
    {
        protected override string GetFunctionUrl()
        {
            return "status/v3/shard-data";
        }

        //

        public async Task<ShardStatus> Get(Region region)
        {
            return await FetchAsync(region, "?");
        }
    }
}
