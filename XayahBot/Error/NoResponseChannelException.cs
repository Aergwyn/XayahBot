using System;

namespace XayahBot.Error
{
    public class NoResponseChannelException : Exception
    {
        public NoResponseChannelException() : base("Channel could not be found.")
        {
        }
    }
}
