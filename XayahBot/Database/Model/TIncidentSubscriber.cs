namespace XayahBot.Database.Model
{
    public class TIncidentSubscriber
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public string ChannelName { get; set; }
    }
}
