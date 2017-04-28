using System;
using XayahBot.Utility;

namespace XayahBot.API
{
    public class CacheEntry
    {
        public object Data { get; private set; }
        private DateTime ExpirationTime { get; set; }

        public CacheEntry(object data)
        {
            this.Data = data;
            this.ExpirationTime = DateTime.UtcNow.AddHours(int.Parse(Property.DataLongevity.Value));
        }

        public bool IsExpired()
        {
            if (DateTime.UtcNow > this.ExpirationTime)
            {
                return true;
            }
            return false;
        }
    }
}
