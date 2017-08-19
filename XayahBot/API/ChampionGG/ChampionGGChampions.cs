using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XayahBot.API.ChampionGG.Model;
using XayahBot.Utility;

namespace XayahBot.API.ChampionGG
{
    public class ChampionGGChampions : ChampionGGApi
    {
        protected override DateTime GetDataExpirationTime()
        {
            DateTime now = DateTime.UtcNow;
            return new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(1);
        }

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
                    tempList.AddRange(await this.GetAsync<List<ChampionStatsDto>>(request));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                if (tempList.Count == 0)
                {
                    break;
                }
                data.AddRange(tempList);
            }
            return data;
        }
    }
}
