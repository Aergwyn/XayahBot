using System;

namespace XayahBot.Database.Error
{
    public class NotExistingException : Exception
    {
        public NotExistingException() : base("Object could not be found.")
        {
        }
    }
}
