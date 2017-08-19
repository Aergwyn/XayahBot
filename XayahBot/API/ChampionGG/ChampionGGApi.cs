using System;
using Newtonsoft.Json;
using XayahBot.API.ChampionGG.Error;
using XayahBot.Utility;

namespace XayahBot.API.ChampionGG
{
    public abstract class ChampionGGApi : Api
    {
        protected override string GetApiKey()
        {
            string apiKey = FileReader.GetFirstLine(Property.FilePath.Value + Property.FileChampionGGApiKey.Value);
            return $"api_key={apiKey}";
        }

        protected override string GetBaseUrl()
        {
            return "http://api.champion.gg/v2";
        }

        protected override string CreateErrorMessage(string content)
        {
            string errorMessage = string.Empty;
            try
            {
                ErrorResponseDto error = JsonConvert.DeserializeObject<ErrorResponseDto>(content);
                errorMessage = $"{error.Error.Code} {error.Error.Message}";
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return errorMessage;
        }
    }
}
