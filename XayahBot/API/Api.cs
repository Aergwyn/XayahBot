using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XayahBot.Error;
using XayahBot.Utility;

namespace XayahBot.API
{
    public abstract class Api
    {
        private static SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private static Dictionary<string, CacheEntry> _cache = new Dictionary<string, CacheEntry>();

        protected abstract string GetApiKey();

        protected abstract string GetBaseUrl();

        protected abstract string CreateErrorMessage(string content);

        // ---

        protected async Task<T> GetAsync<T>(ApiRequest request, DateTime expirationTime = default(DateTime), bool logCall = false)
        {
            T result = default(T);
            await _lock.WaitAsync();
            try
            {
                NoApiResultException noResultError = null;
                CacheEntry cacheData = this.GetFromCache(request.CacheId);
                try
                {
                    if (cacheData == null || cacheData.IsExpired())
                    {
                        result = await this.GetFromApiAsync<T>(request, logCall);
                    }
                }
                catch (NoApiResultException ex)
                {
                    Logger.Error(ex);
                    noResultError = ex;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                if (EqualityComparer<T>.Default.Equals(result, default(T)))
                {
                    if (cacheData != null)
                    {
                        result = (T)cacheData.Data;
                    }
                    else
                    {
                        throw noResultError ?? new NoApiResultException();
                    }
                }
                else
                {
                    _cache.Remove(request.CacheId);
                    if (EqualityComparer<DateTime>.Default.Equals(expirationTime, default(DateTime)))
                    {
                        expirationTime = TimeUtil.Now();
                    }
                    _cache.Add(request.CacheId, new CacheEntry(result, expirationTime));
                }
            }
            finally
            {
                _lock.Release();
            }
            return result;
        }

        private CacheEntry GetFromCache(string cacheId)
        {
            if (_cache.TryGetValue(cacheId, out CacheEntry cacheData))
            {
                return cacheData;
            }
            return null;
        }

        private async Task<T> GetFromApiAsync<T>(ApiRequest request, bool logCall)
        {
            T result = default(T);
            using (HttpClient client = this.SetupHttpClient())
            {
                HttpResponseMessage response = null;
                string url = this.BuildUrl(request, logCall);
                response = await client.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync() ?? string.Empty;
                if (!response.IsSuccessStatusCode)
                {
                    throw new NoApiResultException(this.CreateErrorMessage(content));
                }
                result = JsonConvert.DeserializeObject<T>(content);
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

        private string BuildUrl(ApiRequest request, bool logCall)
        {
            string url = $"{this.GetBaseUrl()}/{request.Resource}?{string.Join("&", request.Arguments)}";
            if (request.Arguments.Count() > 0)
            {
                url += "&";
            }
            if (logCall)
            {
                Logger.Debug($"Calling API: {url}");
            }
            return url + this.GetApiKey();
        }
    }
}
