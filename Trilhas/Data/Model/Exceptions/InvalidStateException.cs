using System;

namespace Trilhas.Data.Model.Exceptions
{
    public class InvalidStateException : Exception
    {
        public InvalidStateException(string Message) : base(Message)
        {
        }
    }
}
