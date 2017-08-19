using System;

namespace XayahBot.Error
{
    public class NoApiResultException : Exception
    {
        public NoApiResultException() : base("There is nothing to return.")
        {
        }

        public NoApiResultException(string message) : base(message)
        {
        }
    }
}
