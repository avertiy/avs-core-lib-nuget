using System;

namespace AVS.CoreLib
{
    public class AppException : Exception
    {
        public string Hint { get; set; }
        public AppException(string message) : base(message)
        {
        }

        public AppException(string message, string hint) : base(message)
        {
            Hint = hint;
        }

        public AppException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
