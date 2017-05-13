using System;

namespace XayahBot.API.Error
{
    public class ErrorResponseException : Exception
    {
        public ErrorResponseException(string message) : base(message)
        {
        }
    }
}
