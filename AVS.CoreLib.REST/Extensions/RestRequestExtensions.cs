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
            var requestBody = request.HttpMethod != "GET" ? request.Body.ToHttpQueryString(orderParameters) : string.Empty;
            return requestBody;
        }

        public static string GetQueryString(this IRequest request, bool orderParameters = false)
        {
            var requestBody = request.HttpMethod == "GET" ? request.Query.ToHttpQueryString(orderParameters) : string.Empty;
            return requestBody;
        }

        public static string GetFullUrl(this IRequest request, bool orderParameters = true)
        {
            var url = request.BaseUrl + request.Path;
            if (request.HttpMethod == "GET" && request.Query != null && request.Query.Any())
                url = UrlHelper.Combine(url, request.Query.ToHttpQueryString(orderParameters));
            return url;
        }

        public static Uri GetUri(this IRequest request, bool orderParameters = true)
        {
            var url = request.BaseUrl + request.Path;

            if (request.HttpMethod == "GET" && request.Query != null && request.Query.Any())
            {
                var queryString = request.Query.ToHttpQueryString(orderParameters);
                url = UrlHelper.Combine(url, queryString);
            }

            return new Uri(url);
        }

        public static void AddQueryParams(this IRequest request, IDictionary<string, object> parameters)
        {
            foreach (var kp in parameters)
            {
                request.Query.Add(kp.Key, kp.Value);
            }
        }

        public static void AddBodyParams(this IRequest request, IDictionary<string, object> parameters)
        {
            foreach (var kp in parameters)
            {
                request.Body.Add(kp.Key, kp.Value);
            }
        }
    }
}