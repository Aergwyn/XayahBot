using System;
using XayahBot.Utility;

namespace XayahBot.API
{
    public class CacheEntry
    {
        public object Data { get; private set; }
        private DateTime ExpirationTime { get; set; }

        public CacheEntry(object data, DateTime expirationTime)
        {
            this.Data = data;
            this.ExpirationTime = expirationTime;
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
