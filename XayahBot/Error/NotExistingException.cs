using System;

namespace XayahBot.Error
{
    public class NotExistingException : Exception
    {
        public NotExistingException() : base("Object not found.")
        {
        }
    }
}
