#nullable enable
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.Extensions.Web;
using AVS.CoreLib.REST.Extensions;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.REST.Clients
{
    public class RequestMessageBuilder : IRequestMessageBuilder
    {
        public bool UseTonce { get; set; }
        public bool OrderQueryStringParameters { get; set; } = true;
        public IAuthenticator? Authenticator { get; set; }

        public const string JSON_CONTENT_HEADER = "application/json";

        public RequestMessageBuilder()
        {
        }

        public RequestMessageBuilder(IAuthenticator authenticator)
        {
            Authenticator = authenticator;
        }

        public HttpRequestMessage Build(IRequest input)
        {
            try
            {
                OnHttpRequestMessageCreating(input);
                var requestMessage = CreateHttpRequestMessage(input);
                OnHttpRequestMessageCreated(requestMessage);
                return requestMessage;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Build HttpRequestMessage failed", ex);
            }
        }

        protected virtual void OnHttpRequestMessageCreated(HttpRequestMessage requestMessage)
        {
        }

        protected virtual void OnHttpRequestMessageCreating(IRequest request)
        {
            if (request.AuthType == AuthType.ApiKey)
            {
                if (UseTonce)
                    request.Data.Add("tonce", NonceHelper.GetTonce());
                else
                    request.Data.Add("nonce", NonceHelper.GetNonce());
            }
        }

        protected virtual HttpRequestMessage CreateHttpRequestMessage(IRequest input)
        {
            var url = input.GetFullUrl(OrderQueryStringParameters);
            var httpMethod = new HttpMethod(input.Method);
            var requestMessage = new HttpRequestMessage(httpMethod, url);
            var queryString = input.Data.ToHttpQueryString(orderBy: OrderQueryStringParameters);
            requestMessage.Content = new StringContent(queryString);
            AddHeaders(requestMessage, queryString);
            return requestMessage;
        }

        protected virtual void AddHeaders(HttpRequestMessage requestMessage, string queryString)
        {
            requestMessage.Headers.AcceptApplicationJsonContent();
            if (Authenticator == null)
                return;

            requestMessage.Headers.Add("Key", Authenticator.PublicKey);
            var signature = Authenticator.Sign(queryString);
            requestMessage.Headers.Add("Sign", signature.ToBase64String());
        }
    }

    public static class HttpRequestHeadersExtenions
    {
        /// <summary>
        /// set headers.Accept "application/json"
        /// </summary>
        /// <param name="headers"></param>
        public static void AcceptApplicationJsonContent(this HttpRequestHeaders headers)
        {
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue(RequestMessageBuilder.JSON_CONTENT_HEADER));
        }
    }
}