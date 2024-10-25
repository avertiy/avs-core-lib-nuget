#nullable enable
using System;
using System.Net.Http;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Extensions.Web;
using AVS.CoreLib.REST.Extensions;

namespace AVS.CoreLib.REST.RequestBuilders
{
    /// <summary>
    /// represent a simple <see cref="IRequestMessageBuilder"/> implementation 
    /// to deal with public REST API where signing (adding an authenticated signature) is not required
    /// </summary>
    public sealed class PublicRequestMessageBuilder : IPublicRequestMessageBuilder
    {
        public bool OrderQueryStringParameters { get; set; } = true;
        public bool UseMediaTypeApplicationJson { get; set; } = true;
        public HttpRequestMessage Build(IRequest request)
        {
            if (request.AuthType == AuthType.ApiKey)
                throw new ArgumentException($"AuthType expected be {AuthType.None}");

            try
            {
                var url = request.GetFullUrl(OrderQueryStringParameters);
                var httpMethod = new HttpMethod(request.HttpMethod);
                var requestMessage = new HttpRequestMessage(httpMethod, url);
                var queryString = request.Data.ToHttpQueryString(orderBy: OrderQueryStringParameters);

                if (httpMethod != HttpMethod.Get)
                    requestMessage.Content = new StringContent(queryString);

                AddHeaders(requestMessage, request);

                return requestMessage;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Build HttpRequestMessage failed", ex);
            }
        }

        private void AddHeaders(HttpRequestMessage requestMessage, IRequest request)
        {
            if (UseMediaTypeApplicationJson)
                requestMessage.Headers.AcceptApplicationJsonContent();

            if (request.Headers == null || request.Headers.Count == 0)
                return;

            foreach (var kp in request.Headers)
                requestMessage.Headers.Add(kp.Key, kp.Value);
        }
    }
}