using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XayahBot.API.ChampionGG.Model;
using XayahBot.Utility;

namespace XayahBot.API.ChampionGG
{
    public class ChampionGGChampions : ChampionGGApi
    {
        public async Task<List<ChampionStatsDto>> GetChampionsAsync()
        {
            int batchSize = 200;
            List<ChampionStatsDto> data = new List<ChampionStatsDto>();
            for (int rounds = 0; true; rounds++)
            {
                List<ChampionStatsDto> tempList = new List<ChampionStatsDto>();
                try
                {
                    ApiRequest request = new ApiRequest("champions", $"limit={batchSize}", $"skip={batchSize * rounds}");
                    tempList.AddRange(await this.GetAsync<List<ChampionStatsDto>>(request, TimeUtil.InDays(1)));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                data.AddRange(tempList);
                if (tempList.Count < batchSize)
                {
                    break;
                }
            }
            return data;
        }
    }
}
