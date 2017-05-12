namespace XayahBot.Database.Model
{
    public class TMessage
    {
        public int Id { get; set; }
        public ulong MessageId { get; set; }
        public ulong ChannelId { get; set; }
        public virtual TIncident Incident { get; set; }
    }
}
