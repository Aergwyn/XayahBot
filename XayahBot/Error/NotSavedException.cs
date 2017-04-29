using System;

namespace XayahBot.Error
{
    public class NotSavedException : Exception
    {
        public NotSavedException() : base("Change was not saved.")
        {
        }
    }
}
