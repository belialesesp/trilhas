using System;

namespace Trilhas.Data.Model.Exceptions
{
    public class TrilhasException : Exception
    {
        public TrilhasException(string Message): base(Message)
        {
        }
    }
}
