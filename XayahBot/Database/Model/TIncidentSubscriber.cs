namespace XayahBot.Database.Model
{
    public class TIncidentSubscriber
    {
        public long Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
    }
}
