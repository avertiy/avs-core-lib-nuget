using System;
using System.Collections.Generic;
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
            var requestBody = request.HttpMethod != "GET" && request.Data != null ? request.Data.ToHttpQueryString(orderParameters) : string.Empty;
            return requestBody;
        }

        public static string GetQueryString(this IRequest request, bool orderParameters = false)
        {
            var requestBody = request.HttpMethod == "GET" && request.Data != null ? request.Data.ToHttpQueryString(orderParameters) : string.Empty;
            return requestBody;
        }

        public static string GetFullUrl(this IRequest request, bool orderParameters = true)
        {
            var url = request.BaseUrl + request.Path;
            if (request.HttpMethod == "GET" && request.Data != null && request.Data.Any())
                url = UrlHelper.Combine(url, request.Data.ToHttpQueryString(orderParameters));
            return url;
        }

        public static Uri GetUri(this IRequest request, bool orderParameters = true)
        {
            var url = request.BaseUrl + request.Path;

            if (request.HttpMethod == "GET" && request.Data != null && request.Data.Any())
            {
                var queryString = request.Data.ToHttpQueryString(orderParameters);
                url = UrlHelper.Combine(url, queryString);
            }

            return new Uri(url);
        }

        public static void AddParams(this IRequest request, IDictionary<string, object> parameters)
        {
            foreach (var kp in parameters)
            {
                request.Data.Add(kp.Key, kp.Value);
            }
        }
    }
}