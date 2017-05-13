using System;

namespace XayahBot.Error
{
    public class UnknownTypeException : Exception
    {
        public UnknownTypeException() : base("This type could not be found.")
        {

        }
    }
}
