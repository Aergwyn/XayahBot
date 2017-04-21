using System;
using System.Collections.Generic;
using System.Linq;
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

        private static DateTime _lastRawChampionListRequest;
        private static ChampionListDto _rawChampionList; // Just to get IDs

        private static Dictionary<int, DateTime> _lastChampionsRequests;
        private static List<ChampionDto> _champions = new List<ChampionDto>(); // Contains detailed data

        //

        public static async Task<ChampionListDto> GetChampionListAsync()
        {
            await _syncLock.WaitAsync();
            try
            {
                if (_rawChampionList == null || _lastRawChampionListRequest.AddHours(int.Parse(Property.DataLongevity.Value)) < DateTime.UtcNow)
                {
                    _rawChampionList = await new ChampionListController().Get();
                    _lastRawChampionListRequest = DateTime.UtcNow;
                }
                return _rawChampionList.Copy(); // Try to return a copy to keep our data safe from changes
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
                if (_lastChampionsRequests == null)
                {
                    _lastChampionsRequests = new Dictionary<int, DateTime>();
                }
                ChampionDto champion = _champions.FirstOrDefault(x => x.Id.Equals(id)); // Look if we got the data already
                _lastChampionsRequests.TryGetValue(id, out DateTime lastUpdate);
                if (champion == null || (lastUpdate != null && lastUpdate.AddHours(int.Parse(Property.DataLongevity.Value)) < DateTime.UtcNow))
                {
                    if (champion != null)
                    {
                        _champions.Remove(champion); // Remove if we are here to update existing
                    }
                    champion = await new ChampionController().Get(id);
                    if (champion != null)
                    {
                        _champions.Add(champion); // Only add if we actually got data
                    }
                    if (!_lastChampionsRequests.ContainsKey(id)) // Add/Update timestamp
                    {
                        _lastChampionsRequests.Add(id, DateTime.UtcNow);
                    }
                    else
                    {
                        _lastChampionsRequests[id] = DateTime.UtcNow;
                    }
                }
                return champion.Copy(); // Try to return a copy to keep our data safe from changes
            }
            finally
            {
                _syncLock.Release();
            }
        }
    }
}