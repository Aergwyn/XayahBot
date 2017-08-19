namespace XayahBot.API.ChampionGG.Error
{
    public class ErrorDto
    {
        public int Code { get; set; }
        public string Method { get; set; }
        public string Route { get; set; }
        public string Message { get; set; }
    }
}
