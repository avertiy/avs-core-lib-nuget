using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Web;
using AVS.CoreLib.REST.Helpers;
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

        public override string ToString()
        {
            return $"{Method} {BaseUrl}{Path}{QueryString.From(Data)}";
        }
    }

    public static class RequestExtensions
    {
        public static string GetFullUrl(this IRequest request)
        {
            var url = request.BaseUrl + request.Path;
            if (request.Method == "GET" && request.Data != null && request.Data.Any())
                url = UrlHelper.Combine(url, request.Data.ToHttpQueryString());
            return url;
        }
    }
}