namespace XayahBot.Database.Model
{
    public class TLeaderboardEntry
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public string UserName { get; set; }
        public int Answers { get; set; }
    }
}
