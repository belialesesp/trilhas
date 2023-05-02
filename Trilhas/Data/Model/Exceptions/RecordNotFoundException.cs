using System;

namespace Trilhas.Data.Model.Exceptions
{
    public class RecordNotFoundException : Exception
    {
        public RecordNotFoundException(string Message) : base(Message)
        {
        }
    }
}
