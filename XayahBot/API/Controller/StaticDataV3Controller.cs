namespace XayahBot.API.Controller
{
    public abstract class StaticDataV3Controller<T> : AbstractController<T>
    {
        protected override string GetBaseUrl()
        {
            return "https://euw1.api.riotgames.com/lol/static-data/v3/";
        }
    }
}
