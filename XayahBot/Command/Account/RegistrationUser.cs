using System;
using Discord;
using XayahBot.API.Riot;

namespace XayahBot.Command.Account
{
    public class RegistrationUser
    {
        public string Name { get; set; }
        public Region Region { get; set; }
        public string Code { get; set; }
        public ulong UserId { get; set; }
        public long SummonerId { get; set; }
        public DateTime ExpirationTime { get; }

        public RegistrationUser()
        {
            this.ExpirationTime = DateTime.UtcNow.AddMinutes(10);
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
