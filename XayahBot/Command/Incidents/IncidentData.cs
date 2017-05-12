using System;
using System.Collections.Generic;

namespace XayahBot.Command.Incidents
{
    public class IncidentData
    {
        public long Id { get; set; }
        public string Region { get; set; }
        public string Service { get; set; }
        public string Status { get; set; }
        public List<UpdateData> Updates { get; set; } = new List<UpdateData>();
        public DateTime LastUpdate { get; set; }
    }
}
