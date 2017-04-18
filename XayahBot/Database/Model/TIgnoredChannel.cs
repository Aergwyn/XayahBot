namespace XayahBot.Database.Model
{
    public class TIgnoredChannel
    {
        public int Id { get; set; }
        public ulong Guild { get; set; }
        public ulong ChannelId { get; set; }
        public string ChannelName { get; set; }
    }
}
