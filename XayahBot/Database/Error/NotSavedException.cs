using System;

namespace XayahBot.Database.Error
{
    public class NotSavedException : Exception
    {
        public NotSavedException() : base("Object state was not saved.")
        {
        }
    }
}
