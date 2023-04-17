using System.Linq;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Extensions.Web;
using AVS.CoreLib.REST.Helpers;

namespace AVS.CoreLib.REST.Extensions
{
    public static class RestRequestExtensions
    {
        public static string GetAbsolutePath(this IRequest request)
        {
            return request.BaseUrl + request.Path;
        }

        public static string GetRequestBody(this IRequest request, bool orderParameters = false)
        {
            var requestBody = request.Method == "GET" ? "" : request.Data.ToHttpQueryString(orderParameters);
            return requestBody;
        }

        public static string GetQueryString(this IRequest request, bool orderParameters = false)
        {
            var requestBody = request.Method == "GET" ? request.Data.ToHttpQueryString(orderParameters) : "";
            return requestBody;
        }

        public static string GetFullUrl(this IRequest request, bool orderParameters = true)
        {
            var url = request.BaseUrl + request.Path;
            if (request.Method == "GET" && request.Data != null && request.Data.Any())
                url = UrlHelper.Combine(url, request.Data.ToHttpQueryString(orderParameters));
            return url;
        }

        //public static string GetPayload(this IRequest request, string separator = "", bool orderParameters = false)
        //{
        //    string payload;
        //    if (request.Method == "GET")
        //    {
        //        payload = $"{request.Method}{separator}{request.BaseUrl}{separator}{request.Path}?{request.Data.ToHttpQueryString(orderParameters)}";
        //    }
        //    else
        //    {
        //        payload = $"{request.Method}{separator}{request.BaseUrl}{separator}{request.Path}{separator}{request.Data.ToHttpQueryString(orderParameters)}";
        //    }
            
        //    return payload;
        //}
    }
}