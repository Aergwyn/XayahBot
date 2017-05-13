using System;

namespace XayahBot.Error
{
    public class NoChannelException : Exception
    {
        public NoChannelException() : base("Channel could not be found.")
        {
        }
    }
}
