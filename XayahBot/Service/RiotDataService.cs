using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XayahBot.API.Controller;
using XayahBot.API.Model;
using XayahBot.Utility;

namespace XayahBot.Service
{
    public static class RiotDataService
    {
        private static SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);
        private static DateTime LastRawChampionListRequest { get; set; }
        private static ChampionListDto RawChampionList { get; set; } // Just to get IDs

        private static Dictionary<int, DateTime> LastChampionsRequests { get; set; }
        private static List<ChampionDto> Champions { get; set; } // Contains detailed data

        //

        public static async Task<ChampionListDto> GetChampionListAsync()
        {
            await _syncLock.WaitAsync();
            try
            {
                if (RawChampionList == null || LastRawChampionListRequest.AddHours(int.Parse(Property.DataLongevity.Value)) < DateTime.UtcNow)
                {
                    RawChampionList = await new ChampionListController().Get();
                    LastRawChampionListRequest = DateTime.UtcNow;
                }
                return RawChampionList.Copy(); // Try to return a copy to keep our data save from changes
            }
            finally
            {
                _syncLock.Release();
            }
        }

        public static async Task<ChampionDto> GetChampionDetailsAsync(int id)
        {
            await _syncLock.WaitAsync();
            try
            {
                if (Champions == null)
                {
                    Champions = new List<ChampionDto>();
                }
                if (LastChampionsRequests == null)
                {
                    LastChampionsRequests = new Dictionary<int, DateTime>();
                }
                ChampionDto champion = Champions.FirstOrDefault(x => x.Id.Equals(id)); // Look if we got the data already
                LastChampionsRequests.TryGetValue(id, out DateTime lastUpdate);
                if (champion == null || (lastUpdate != null && lastUpdate.AddHours(int.Parse(Property.DataLongevity.Value)) < DateTime.UtcNow))
                {
                    if (champion != null)
                    {
                        Champions.Remove(champion); // Remove if we are here to update existing
                    }
                    champion = await new ChampionController().Get(id);
                    if (champion != null)
                    {
                        Champions.Add(champion); // Only add if we actually got data
                    }
                    if (!LastChampionsRequests.ContainsKey(id)) // Add/Update timestamp
                    {
                        LastChampionsRequests.Add(id, DateTime.UtcNow);
                    }
                    else
                    {
                        LastChampionsRequests[id] = DateTime.UtcNow;
                    }
                }
                return champion.Copy(); // Try to return a copy to keep our data save from changes
            }
            finally
            {
                _syncLock.Release();
            }
        }
    }
}