#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
#pragma warning disable CS1591 
namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IRequest
    {
        AuthType AuthType { get; set; }
        string HttpMethod { get; set; }
        string BaseUrl { get; set; }
        string Path { get; set; }
        Dictionary<string, string>? Headers { get; set; }
        IParametersCollection Data { get; set; }

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

        /// <summary>
        /// A function to run after configuration of the HTTP request (e.g. set last minute headers)
        /// </summary>
        Action<HttpRequestMessage>? OnRequestMessageReady { get; set; }

        int GetRetryDelay();
    }

    public interface IParametersCollection : IDictionary<string, object>
    {
        bool AddOptional<T>(string key, T? value);
        bool AddOptional(string key, string? value);
        bool AddOptionalEnum<T>(string key, T? value, Func<T, string> converter) where T : Enum;

        void AddRequired(string key, string? value);
        void AddRequired(string key, int value);
        
        void AddRequired<T>(string key, T value) where T : struct;
    }
}

#pragma warning restore CS1591