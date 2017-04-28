#pragma warning disable 4014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XayahBot.Utility;

namespace XayahBot.API
{
    public abstract class CachedApi
    {
        private static SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private static Dictionary<string, CacheEntry> _cache = new Dictionary<string, CacheEntry>();

        //

        protected abstract string GetApiKey();

        protected abstract string GetBaseUrl();

        //

        protected async Task<T> GetAsync<T>(ApiRequest request)
        {
            T result = default(T);
            await _lock.WaitAsync();
            try
            {
                result = this.GetFromCache<T>(request.CacheId);
                if (result == null)
                {
                    result = await this.GetFromApiAsync<T>(request);
                    _cache.Add(request.CacheId, new CacheEntry(result));
                }
            }
            finally
            {
                _lock.Release();
            }
            return result;
        }

        //

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

        private async Task<T> GetFromApiAsync<T>(ApiRequest request)
        {
            T result = default(T);
            using (HttpClient client = this.SetupHttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(this.BuildUrl(request));
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync() ?? string.Empty;
                    result = JsonConvert.DeserializeObject<T>(responseString);
                }
                catch (Exception ex)
                {
                    Logger.Warning(ex.Message, ex);
                }
            }
            return result;
        }

        private HttpClient SetupHttpClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        private string BuildUrl(ApiRequest request)
        {
            string url = $"{this.GetBaseUrl()}/{request.Resource}?{string.Join("&", request.Arguments)}";
            if (request.Arguments.Count() > 0)
            {
                url += "&";
            }
            Logger.Debug($"Calling API: {url}");
            return url + this.GetApiKey();
        }
    }
}
