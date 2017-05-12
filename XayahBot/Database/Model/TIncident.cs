using System;
using System.Collections.Generic;

namespace XayahBot.Database.Model
{
    public class TIncident
    {
        public int Id { get; set; }
        public long IncidentId { get; set; }
        public DateTime LastUpdate { get; set; }
        public virtual ICollection<TMessage> Messages { get; set; } = new List<TMessage>();
    }
}
