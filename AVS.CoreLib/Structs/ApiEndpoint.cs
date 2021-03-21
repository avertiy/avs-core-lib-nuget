using System;

namespace AVS.CoreLib.Utilities
{
    /// <summary>
    /// represents $"{BaseAddress}/{Api}/{Version}/{Command}"
    /// in case you need different url template e.g. https://poloniex.com/public?command=returnOrderBook
    /// create another struct it is only used to BuildUrl
    /// </summary>
    public struct ApiEndpoint 
    {
        public string Command { get; set; }
        public string Url { get; }
        public string Method { get; set; }
        public EndpointSecurityType SecurityType { get; set; }

        public ApiEndpoint(string url)
        {
            Url = url;
            Method = "GET";
            SecurityType = EndpointSecurityType.None;
            Command = null;
        }

        public ApiEndpoint(string url, string command="", string method = "GET", 
            EndpointSecurityType securityType = EndpointSecurityType.None)
        {
            Command = command;
            Url = url;
            Method = method;
            SecurityType = securityType;
        }

        public ApiEndpoint(string baseAddress, string api, string version, string relative, string command, 
            string method = "GET", EndpointSecurityType securityType = EndpointSecurityType.None)
        {
            Command = command;
            Url = $"{baseAddress}{api}{version}{relative}{Command}";
            Method = method;
            SecurityType = securityType;
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
            return $"{Method} {Url}{Command} ({SecurityType})";
        }
    }

    public enum EndpointSecurityType
    {
        None = 0,
        Signed,
    }
}