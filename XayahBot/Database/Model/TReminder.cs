using System;

namespace XayahBot.Database.Model
{
    public class TReminder : IIdentifiable
    {
        public long Id { get; set; } = 0;
        public ulong UserId { get; set; }
        public DateTime ExpirationTime { get; set; }
        public string Message { get; set; }

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
