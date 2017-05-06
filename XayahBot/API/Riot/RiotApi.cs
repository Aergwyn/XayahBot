using XayahBot.Utility;

namespace XayahBot.API.Riot
{
    public abstract class RiotApi : Api
    {
        protected abstract string GetService();

        protected abstract string GetVersion();

        // ---

        private Region _region;
        private string _apiKey;

        protected RiotApi(Region region)
        {
            this._region = region ?? Region.EUW;
            this._apiKey = FileReader.ReadFirstLine(Property.FilePath.Value + Property.FileRiotApiKey.Value);
        }

        protected override string GetApiKey()
        {
            return $"api_key={this._apiKey}";
        }

        protected override string GetBaseUrl()
        {
            return $"https://{this._region.Platform}.api.riotgames.com/lol/{this.GetService()}/{this.GetVersion()}";
        }
    }
}
