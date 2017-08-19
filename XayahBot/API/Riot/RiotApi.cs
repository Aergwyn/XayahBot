using System;
using Newtonsoft.Json;
using XayahBot.API.Riot.Error;
using XayahBot.Utility;

namespace XayahBot.API.Riot
{
    public abstract class RiotApi : Api
    {
        protected Region _region;

        protected RiotApi(Region region)
        {
            this._region = region;
        }

        public Region GetRegion()
        {
            return this._region;
        }

        protected override string GetApiKey()
        {
            string apiKey = FileReader.GetFirstLine(Property.FilePath.Value + Property.FileRiotApiKey.Value);
            return $"api_key={apiKey}";
        }

        protected override string GetBaseUrl()
        {
            return $"https://{this._region.Platform}.api.riotgames.com/lol/{this.GetService()}/{this.GetVersion()}";
        }

        protected override string CreateErrorMessage(string content)
        {
            string errorMessage = string.Empty;
            try
            {
                ErrorResponseDto error = JsonConvert.DeserializeObject<ErrorResponseDto>(content);
                errorMessage = $"{error.Status.StatusCode} {error.Status.Message}";
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return errorMessage;
        }

        protected abstract string GetService();

        protected abstract string GetVersion();
    }
}
