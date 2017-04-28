#pragma warning disable 4014

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;
using XayahBot.Utility;
using System.Linq;

namespace XayahBot.API.Controller
{
    public abstract class AbstractController<T>
    {
        protected abstract string GetFunctionUrl();

        //

        private string _apiKey;

        private string GetApiKeyAsync()
        {
            if (string.IsNullOrWhiteSpace(this._apiKey))
            {
                this._apiKey = File.ReadLines(Property.FilePath.Value + Property.FileRiotApiKey.Value).ElementAt(0); // I'm not gonna show it
            }
            return $"api_key={_apiKey}";
        }

        //

        protected async Task<T> FetchAsync(Region region)
        {
            return await FetchAsync(region, string.Empty);
        }

        protected async Task<T> FetchAsync(Region region, string dataString)
        {
            if (dataString == null) // I don't care about the dataString but it can't be null after this point
            {
                dataString = string.Empty;
            }
            string responseString = string.Empty;
            T resultObject = default(T);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Set Base-URI and tell the client which form of data we expect
                    client.BaseAddress = new Uri($"https://{region.ApiName}.api.riotgames.com/lol/{GetFunctionUrl()}");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Request json
                    Logger.Debug($"Requesting data from \"{client.BaseAddress + dataString}\".");
                    HttpResponseMessage response = await client.GetAsync(dataString + GetApiKeyAsync());
                    response.EnsureSuccessStatusCode();
                    // Set response and deserialize json to generic type T
                    responseString = await response.Content.ReadAsStringAsync();
                    resultObject = JsonConvert.DeserializeObject<T>(responseString);
                }
                catch (Exception ex)
                {
                    await Logger.Warning(ex.Message, ex);
                    resultObject = default(T);
                }
                return resultObject;
            }
        }
    }
}
