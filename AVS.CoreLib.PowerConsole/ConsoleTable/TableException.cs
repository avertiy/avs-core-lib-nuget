using System;
using System.Runtime.Serialization;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public class TableException : Exception
    {
        public TableException()
        {
        }

        protected TableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public TableException(string message) : base(message)
        {
        }

        public TableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}