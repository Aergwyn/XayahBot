using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XayahBot.API
{
    public abstract class CachedApi : Api
    {
        protected abstract DateTime GetDataExpirationTime();

        //

        private static SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private static Dictionary<string, CacheEntry> _cache = new Dictionary<string, CacheEntry>();

        new protected async Task<T> GetAsync<T>(ApiRequest request)
        {
            T result = default(T);
            await _lock.WaitAsync();
            try
            {
                result = this.GetFromCache<T>(request.CacheId);
                if (result == null)
                {
                    result = await base.GetAsync<T>(request);
                    _cache.Add(request.CacheId, new CacheEntry(result, this.GetDataExpirationTime()));
                }
            }
            finally
            {
                _lock.Release();
            }
            return result;
        }

        private T GetFromCache<T>(string cacheId)
        {
            T result = default(T);
            if (_cache.TryGetValue(cacheId, out CacheEntry entry))
            {
                if (entry.IsExpired())
                {
                    _cache.Remove(cacheId);
                }
                else
                {
                    result = (T)entry.Data;
                }
            }
            return result;
        }
    }
}
