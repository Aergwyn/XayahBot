namespace XayahBot.API.ChampionGG.Model
{
    public class ChampionStatsDto
    {
        public double BanRate { get; set; }
        public int ChampionId { get; set; }
        public string Elo { get; set; }
        public int GamesPlayed { get; set; }
        //public Id Id { get; set; }
        public string Patch { get; set; }
        public double PercentRolePlayed { get; set; }
        public double PlayRate { get; set; }
        public string Role { get; set; }
        public double WinRate { get; set; }
    }
}
