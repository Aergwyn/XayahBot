﻿namespace XayahBot.Database.Model
{
    public class TIncidentSubscriber : IIdentifiable
    {
        public long Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }

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
