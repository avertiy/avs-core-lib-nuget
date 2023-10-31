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
        public HttpRequestMessage Build(IRequest input)
        {
            if (input.AuthType == AuthType.ApiKey)
                throw new ArgumentException($"AuthType expected be {AuthType.None}");

            try
            {
                var url = input.GetFullUrl(OrderQueryStringParameters);
                var httpMethod = new HttpMethod(input.Method);
                var requestMessage = new HttpRequestMessage(httpMethod, url);
                var queryString = input.Data.ToHttpQueryString(orderBy: OrderQueryStringParameters);

                if (httpMethod != HttpMethod.Get)
                    requestMessage.Content = new StringContent(queryString);

                if (UseMediaTypeApplicationJson)
                    requestMessage.Headers.AcceptApplicationJsonContent();

                return requestMessage;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Build HttpRequestMessage failed", ex);
            }
        }
    }
}