#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Extensions.Web;
using AVS.CoreLib.REST.Extensions;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.REST.RequestBuilders
{
    /// <summary>
    /// represent RequestMessageBuilder to build authenticated (signed) http request messages
    /// if you need unsigned http request messages to consume public api <see cref="PublicRequestMessageBuilder"/>
    /// </summary>
    public class RequestMessageBuilder : IRequestMessageBuilder
    {
        public bool UseTonce { get; set; }
        public bool OrderQueryStringParameters { get; set; } = true;
        public bool UseMediaTypeApplicationJson { get; set; } = true;
        public IAuthenticator Authenticator { get; set; }

        public RequestMessageBuilder(IAuthenticator authenticator)
        {
            Authenticator = authenticator;
        }

        public HttpRequestMessage Build(IRequest request)
        {
            try
            {
                OnHttpRequestMessageCreating(request);
                var requestMessage = CreateHttpRequestMessage(request);
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
                if (UseTonce)
                    request.Data["tonce"] = NonceHelper.GetTonce();
                else
                    request.Data["nonce"] = NonceHelper.GetNonce().ToString();
        }

        protected virtual HttpRequestMessage CreateHttpRequestMessage(IRequest request)
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

        protected void AddHeaders(HttpRequestMessage requestMessage, IRequest request)
        {
            if (UseMediaTypeApplicationJson)
                requestMessage.Headers.AcceptApplicationJsonContent();

            if (request.Headers == null || request.Headers.Count == 0)
                return;

            foreach (var kp in request.Headers)
                requestMessage.Headers.Add(kp.Key, kp.Value);
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