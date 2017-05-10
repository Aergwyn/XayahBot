namespace XayahBot.Database.Model
{
    public class TIncident
    {
        public int Id { get; set; }
        public long IncidentId { get; set; }
        public string UpdateId { get; set; }
        public ulong MessageId { get; set; }
    }
}
