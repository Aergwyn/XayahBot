#pragma warning disable 4014

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XayahBot.Utility;

namespace XayahBot.API
{
    public abstract class Api
    {
        protected abstract string GetApiKey();

        protected abstract string GetBaseUrl();

        //

        protected async Task<T> GetAsync<T>(ApiRequest request)
        {
            return await this.GetFromApiAsync<T>(request);
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
