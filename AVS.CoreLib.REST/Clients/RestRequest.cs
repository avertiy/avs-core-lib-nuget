#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions.Rest;
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
        public Dictionary<string, object> Data { get; set; } = new();
        public int RateLimit { get; set; } = 1;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int RetryAttempt { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RequestId { get; set; }

        public override string ToString()
        {
            var qs = QueryString.From(Data).ToString();
            var authType = AuthType == AuthType.ApiKey ? " (SIGNED)" : string.Empty;
            var attempt = RetryAttempt > 0 ? $" [RETRY #{RetryAttempt}]" : string.Empty;
            var reqId = RequestId == null ? string.Empty : $" Req.Id #{RequestId}";
            return $"{HttpMethod} {BaseUrl}{Path}{qs}{authType}{attempt}{reqId}".Trim();
        }
    }
}