namespace XayahBot.Database.Model
{
    public class TIncidentMessage : IIdentifiable
    {
        public long Id { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public virtual TIncident Incident { get; set; }

        public bool IsNew()
        {
            if (this.Id > 0)
            {
                return false;
            }
            return true;
        }
    }
}
