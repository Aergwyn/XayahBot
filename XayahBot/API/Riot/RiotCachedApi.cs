using XayahBot.Utility;

namespace XayahBot.API.Riot
{
    public abstract class RiotCachedApi : CachedApi
    {
        protected abstract string GetService();

        protected abstract string GetVersion();

        //

        private Region _region;
        private string _apiKey;

        private FileReader _fileReader = new FileReader();

        protected RiotCachedApi(Region region)
        {
            this._region = region ?? Region.EUW;
            this._apiKey = this._fileReader.ReadFirstLine(Property.FilePath.Value + Property.FileRiotApiKey.Value);
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
