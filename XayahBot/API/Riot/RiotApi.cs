using XayahBot.Utility;

namespace XayahBot.API
{
    public abstract class RiotApi : CachedApi
    {
        private Region _region;
        private string _apiKey;

        private FileReader _fileReader = new FileReader();

        protected RiotApi(Region region)
        {
            this._region = region ?? Region.EUW;
            this._apiKey = this._fileReader.ReadFirstLine(Property.FilePath.Value + Property.FileRiotApiKey.Value);
        }

        //

        protected abstract string GetService();

        protected abstract string GetVersion();

        //

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
