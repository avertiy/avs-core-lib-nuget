#nullable enable
using System;

namespace AVS.CoreLib.REST
{
    public class RateLimitExceededException : Exception
    {
        public string? HttpClientName
        {
            get;
            set;
        }

        public RateLimitExceededException(string message, string? httpClientName = null) : base(message)
        {
            HttpClientName = httpClientName;
        }

        public RateLimitExceededException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}