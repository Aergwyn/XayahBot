using System;

namespace XayahBot.Error
{
    public class AlreadyExistingException : Exception
    {
        public AlreadyExistingException() : base("Object already exists.")
        {
        }
    }
}
