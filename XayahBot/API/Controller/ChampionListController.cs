using System.Threading.Tasks;
using XayahBot.API.Model;
using XayahBot.Utility;

namespace XayahBot.API.Controller
{
    public class ChampionListController : StaticDataV3Controller<ChampionListDto>
    {
        public async Task<ChampionListDto> Get()
        {
            return await FetchAsync(Region.EUW, "champions?dataById=true&");
        }
    }
}
