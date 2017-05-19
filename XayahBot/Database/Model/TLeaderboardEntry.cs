namespace XayahBot.Database.Model
{
    public class TLeaderboardEntry
    {
        public long Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public int Answers { get; set; }
    }
}
