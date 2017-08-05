using XayahBot.Utility;

namespace XayahBot.API.Riot
{
    public abstract class RiotApi : Api
    {
        protected Region _region;
        private string _apiKey;

        protected RiotApi(Region region)
        {
            this._region = region;
            this._apiKey = FileReader.GetFirstLine(Property.FilePath.Value + Property.FileRiotApiKey.Value);
        }

        public Region GetRegion()
        {
            return this._region;
        }

        protected override string GetApiKey()
        {
            return $"api_key={this._apiKey}";
        }

        protected override string GetBaseUrl()
        {
            return $"https://{this._region.Platform}.api.riotgames.com/lol/{this.GetService()}/{this.GetVersion()}";
        }

        protected abstract string GetService();

        protected abstract string GetVersion();
    }
}
