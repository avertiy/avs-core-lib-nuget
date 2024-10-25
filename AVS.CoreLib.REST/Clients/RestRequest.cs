#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.REST.Clients
{
    [DebuggerDisplay("RestRequest {ToString()}")]
    public class RestRequest : IRequest
    {
        public AuthType AuthType { get; set; }
        public string HttpMethod { get; set; } = null!;
        public string BaseUrl { get; set; } = null!;
        public string Path { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string>? Headers { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Timeout { get; set; }
        public int RateLimit { get; set; } = 1;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int RetryAttempt { get; set; }

        /// <summary>
        /// retry delay in milliseconds
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? RetryDelay { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RequestId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Action<HttpRequestMessage>? OnRequestMessageReady { get; set; }

        public int GetRetryDelay()
        {
            if (RetryDelay.HasValue)
                return RetryDelay.Value;

            // Exponential backoff
            return (int)Math.Pow(2, RetryAttempt) * 1000;
        }

        public override string ToString()
        {
            var headers = Headers != null && Headers.Count > 0
                ? $"headers:{Headers.ToKeyValueString()} " 
                : string.Empty;
            var qs = QueryString.From(Data).ToString();
            var authType = AuthType == AuthType.ApiKey ? " (SIGNED)" : string.Empty;
            var attempt = RetryAttempt > 0 ? $" [RETRY #{RetryAttempt}]" : string.Empty;
            var reqId = RequestId == null ? string.Empty : $" Req.Id #{RequestId}";

            return $"{HttpMethod} {headers}{BaseUrl}{Path}{qs}{authType}{attempt}{reqId}".Trim();
        }
    }
}