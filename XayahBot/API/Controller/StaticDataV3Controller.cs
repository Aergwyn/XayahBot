namespace XayahBot.API.Controller
{
    public abstract class StaticDataV3Controller<T> : AbstractController<T>
    {
        protected override string GetFunctionUrl() // EUW Default
        {
            return "static-data/v3/";
        }
    }
}
