using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XayahBot.API.Error;
using XayahBot.Utility;

namespace XayahBot.API
{
    public abstract class Api
    {
        protected abstract string GetApiKey();

        protected abstract string GetBaseUrl();

        // ---

        protected async Task<T> GetAsync<T>(ApiRequest request)
        {
            return await this.GetFromApiAsync<T>(request);
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
                catch (ErrorResponseException erex)
                {
                    Logger.Error(erex.Message);
                    throw erex;
                }
            }
            return result;
        }

        private void CheckResponseStatus(HttpResponseMessage response, string content)
        {
            if (!response.IsSuccessStatusCode)
            {
                ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(content);
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
