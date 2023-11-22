using System.Collections.Generic;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.REST.Clients
{
    public class RestRequest : IRequest
    {
        public AuthType AuthType { get; set; }
        public string Method { get; set; }
        public string BaseUrl { get; set; }
        public string Path { get; set; }
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
        public int RateLimit { get; set; } = 1;
        public override string ToString()
        {
            var authType = AuthType == AuthType.ApiKey ? " (SIGNED)" : string.Empty;
            return $"{Method} {BaseUrl}{Path}{QueryString.From(Data)}{authType}";
        }
    }
}