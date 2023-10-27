#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.Extensions.Web;
using AVS.CoreLib.REST.Extensions;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.REST.Clients
{
    /// <summary>
    /// represent RequestMessageBuilder to build authenticated (signed) http request messages
    /// if you need unsigned http request messages to consume public api <see cref="PublicRequestMessageBuilder"/>
    /// </summary>
    public class RequestMessageBuilder : IRequestMessageBuilder
    {
        public bool UseTonce { get; set; }
        public bool OrderQueryStringParameters { get; set; } = true;
        public IAuthenticator Authenticator { get; set; }

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
                    request.Data["tonce"] = NonceHelper.GetTonce();
                else
                    request.Data["nonce"] = NonceHelper.GetNonce();
            }
        }

        protected virtual HttpRequestMessage CreateHttpRequestMessage(IRequest input)
        {
            var url = input.GetFullUrl(OrderQueryStringParameters);
            var httpMethod = new HttpMethod(input.Method);
            var requestMessage = new HttpRequestMessage(httpMethod, url);
            var queryString = input.Data.ToHttpQueryString(orderBy: OrderQueryStringParameters);

            if (httpMethod != HttpMethod.Get)
            {
                requestMessage.Content = new StringContent(queryString);
            }
            
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

    public static class MediaTypes
    {
        public const string APPLICATION_JSON = "application/json";
        public const string FORM_URL_ENCODED = "application/x-www-form-urlencoded";
    }

    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// set headers.Accept "application/json"
        /// </summary>
        /// <param name="headers"></param>
        public static void AcceptApplicationJsonContent(this HttpRequestHeaders headers)
        {
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.APPLICATION_JSON));
        }

        /// <summary>
        /// when u use request.Content = new StringContent(queryString) the media type by default is text/plain
        /// here by default media type is application/x-www-form-urlencoded
        /// </summary>
        public static void SetContent(this HttpRequestMessage request, string queryString, string mediaType = MediaTypes.FORM_URL_ENCODED)
        {
            request.Content = new StringContent(queryString, Encoding.UTF8, mediaType);
        }

        public static void SetContentAsFormUrlEncodedContent(this HttpRequestMessage request, IDictionary<string, string> data)
        {
            var formData = new FormUrlEncodedContent(data);
            request.Content = formData;
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypes.FORM_URL_ENCODED);
        }
    }
}