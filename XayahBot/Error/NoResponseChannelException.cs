using System;

namespace XayahBot.Error
{
    public class NoResponseChannelException : Exception
    {
        public NoResponseChannelException() : base("Could not reply because no channel could be found!")
        {
        }
    }
}
