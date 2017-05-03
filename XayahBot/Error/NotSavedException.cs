using System;

namespace XayahBot.Error
{
    public class NotSavedException : Exception
    {
        public NotSavedException() : base("Object state was not saved.")
        {
        }
    }
}
