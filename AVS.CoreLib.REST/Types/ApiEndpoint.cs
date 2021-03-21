using System;
using AVS.CoreLib.Abstractions.Rest;

namespace AVS.CoreLib.REST.Types
{
    /// <summary>
    /// represents $"{BaseAddress}/{Api}/{Version}/{Command}"
    /// in case you need different url template e.g. https://poloniex.com/public?command=returnOrderBook
    /// create another struct it is only used to BuildUrl
    /// </summary>
    public struct ApiEndpoint : IEndpoint
    {
        public string Command { get; set; }
        public string Url { get; }
        public string Method { get; set; }
        public AuthType AuthType { get; set; }

        public ApiEndpoint(string url)
        {
            Url = url;
            Method = "GET";
            AuthType = AuthType.None;
            Command = null;
        }

        public ApiEndpoint(string url, string command = "", string method = "GET",
            AuthType authType = AuthType.None)
        {
            Command = command;
            Url = url;
            Method = method;
            AuthType = authType;
        }

        public ApiEndpoint(string baseAddress, string api, string version, string relative, string command,
            string method = "GET", AuthType authType = AuthType.None)
        {
            Command = command;
            Url = $"{baseAddress}{api}{version}{relative}{Command}";
            Method = method;
            AuthType = authType;
        }

        public static implicit operator Uri(ApiEndpoint endpoint)
        {
            return new Uri(endpoint.Url);
        }

        public static explicit operator ApiEndpoint(string url)
        {
            return new ApiEndpoint(url);
        }

        public override string ToString()
        {
            return $"{Method} {Url}{Command} ({AuthType})";
        }
    }
}