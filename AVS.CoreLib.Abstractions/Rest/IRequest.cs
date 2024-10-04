#nullable enable
using System.Collections.Generic;

namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IRequest
    {
        AuthType AuthType { get; set; }
        string HttpMethod { get; set; }
        string BaseUrl { get; set; }
        string Path { get; set; }
        Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// Timeout in milliseconds
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// sets the number of delay portions of a rate limiter,
        /// one portion by default is 50ms        
        /// </summary>
        int RateLimit { get; set; }
        int RetryAttempt { get; set; }
        /// <summary>
        /// optional field
        /// </summary>
        string? RequestId { get; set; }

        int GetRetryDelay();
    }
}