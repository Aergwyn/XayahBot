﻿using System;
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
        protected abstract string GetBaseUrl();

        //

        private string _apiKey;

        private string GetApiKeyAsync()
        {
            if (string.IsNullOrWhiteSpace(this._apiKey))
            {
                this._apiKey = File.ReadLines(Property.FilePath.Value + Property.FileRiotApiKey.Value).ElementAt(0);
            }
            return $"api_key={_apiKey}";
        }

        //

#pragma warning disable 4014 // Intentional
        protected async Task<T> FetchAsync(string dataString)
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
                    client.BaseAddress = new Uri(GetBaseUrl());
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Request json
                    Logger.Log(LogSeverity.Debug, nameof(AbstractController<T>), $"Requesting data from \"{client.BaseAddress + dataString}\".");
                    HttpResponseMessage response = await client.GetAsync(dataString + GetApiKeyAsync());
                    response.EnsureSuccessStatusCode();
                    // Set response and deserialize json to generic type T
                    responseString = await response.Content.ReadAsStringAsync();
                    resultObject = JsonConvert.DeserializeObject<T>(responseString);
                }
                catch (Exception ex)
                {
                    await Logger.Log(new LogMessage(LogSeverity.Warning, nameof(AbstractController<T>), ex.Message, ex));
                    resultObject = default(T);
                }
                return resultObject;
            }
        }
#pragma warning restore 4014
    }
}