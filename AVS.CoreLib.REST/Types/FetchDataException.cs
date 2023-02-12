using System;
using System.Runtime.Serialization;

namespace AVS.CoreLib.REST
{
    public class FetchDataException : Exception
    {
        protected FetchDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public FetchDataException(string message) : base(message)
        {
        }

        public FetchDataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}