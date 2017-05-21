namespace XayahBot.Database.Model
{
    public class TAccount
    {
        public long Id { get; set; }
        public ulong UserId { get; set; }
        public long SummonerId { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
    }
}
