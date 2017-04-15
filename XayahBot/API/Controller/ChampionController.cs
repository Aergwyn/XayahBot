using System.Threading.Tasks;
using XayahBot.API.Model;

namespace XayahBot.API.Controller
{
    class ChampionController : StaticDataV3Controller<ChampionDto>
    {
        public async Task<ChampionDto> Get(int id)
        {
            return await FetchAsync($"champions/{id}?champData=partype,passive,skins,spells,stats,tags&");
        }
    }
}
