using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XayahBot.API.Error;
using XayahBot.Utility;

namespace XayahBot.API
{
    public abstract class Api
    {
        private static SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private static Dictionary<string, CacheEntry> _cache = new Dictionary<string, CacheEntry>();

        protected abstract DateTime GetDataExpirationTime();

        protected abstract string GetApiKey();

        protected abstract string GetBaseUrl();

        // ---

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

        private async Task<T> GetFromApiAsync<T>(ApiRequest request)
        {
            T result = default(T);
            using (HttpClient client = this.SetupHttpClient())
            {
                HttpResponseMessage response = null;
                try
                {
                    response = await client.GetAsync(this.BuildUrl(request));
                    string content = await response.Content.ReadAsStringAsync() ?? string.Empty;
                    this.CheckResponseStatus(response, content);
                    result = JsonConvert.DeserializeObject<T>(content);
                }
                catch (ErrorResponseException ex)
                {
                    Logger.Error(ex.Message);
                    throw ex;
                }
            }
            return result;
        }

        private void CheckResponseStatus(HttpResponseMessage response, string content)
        {
            if (!response.IsSuccessStatusCode)
            {
                ErrorDto error = JsonConvert.DeserializeObject<ErrorDto>(content);
                throw new ErrorResponseException($"{error.Status.StatusCode} {error.Status.Message}");
            }
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
