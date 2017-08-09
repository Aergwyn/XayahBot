using System;
using System.Collections.Generic;

namespace XayahBot.Database.Model
{
    public class TIncident : IIdentifiable
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public string Region { get; set; }
        public DateTime LastUpdate { get; set; }
        public virtual ICollection<TIncidentMessage> Messages { get; set; } = new List<TIncidentMessage>();

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
