﻿namespace XayahBot.Database.Model
{
    public class TPostedIncident
    {
        public int Id { get; set; }
        public long IncidentId { get; set; }
        public string UpdateId { get; set; }
        public string Region { get; set; }
        public ulong MessageId { get; set; }
    }
}
