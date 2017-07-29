using XayahBot.Utility;

namespace XayahBot.API.Riot
{
    public abstract class RiotCachedApi : CachedApi
    {
        protected abstract string GetService();

        protected abstract string GetVersion();

        // ---

        protected Region _region;
        private string _apiKey;

        protected RiotCachedApi(Region region)
        {
            this._region = region;
            this._apiKey = FileReader.GetFirstLine(Property.FilePath.Value + Property.FileRiotApiKey.Value);
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
