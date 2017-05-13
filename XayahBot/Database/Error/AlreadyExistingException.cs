using System;

namespace XayahBot.Database.Error
{
    public class AlreadyExistingException : Exception
    {
        public AlreadyExistingException() : base("Object already exists.")
        {
        }
    }
}
