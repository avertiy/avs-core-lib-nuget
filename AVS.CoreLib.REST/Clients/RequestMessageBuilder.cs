using System;
using System.Net.Http;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.REST.Helpers;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.REST.Clients
{
    public class RequestMessageBuilder : IRequestMessageBuilder
    {
        public bool AddCommandToRequestData { get; set; } = true;
        public bool UseTonce { get; set; }
        public IAuthenticator Authenticator { get; protected set; }
        public HttpRequestMessage Build(IEndpoint endpoint, IPayload data)
        {
            try
            {
                var message = new HttpRequestMessage(new HttpMethod(endpoint.Method), GetUri(endpoint, data))
                {
                    Content = CreateHttpContent(endpoint, data)
                };
                return message;
            }
            catch (Exception ex)
            {
                throw new Exception("Build HttpRequestMessage failed", ex);
            }
        }

        protected virtual Uri GetUri(IEndpoint endpoint, IPayload data)
        {
            var url = endpoint.Url;
            if (data != null)
            {
                url += data.RelativeUrl;
                if (endpoint.Method == "GET")
                {
                    url = UrlHelper.Combine(url, data.ToHttpQueryString());
                }
            }
            return new Uri(url);
        }

        protected virtual HttpContent CreateHttpContent(IEndpoint endpoint, IPayload data)
        {
            OnHttpContentCreating(endpoint, data);
            var queryString = data.ToHttpQueryString();
            HttpContent content = new StringContent(queryString);

            if (endpoint.AuthType == AuthType.ApiKey)
            {
                AddAuthHeaders(content, queryString);
            }
            return content;
        }

        protected virtual void OnHttpContentCreating(IEndpoint endpoint, IPayload data)
        {
            if (endpoint.AuthType == AuthType.ApiKey)
            {
                if (AddCommandToRequestData)
                    data.Add("command", endpoint.Command);
                if (UseTonce)
                    data.Add("tonce", NonceHelper.GetTonce());
                else
                    data.Add("nonce", NonceHelper.GetNonce());
            }
        }

        protected virtual void AddAuthHeaders(HttpContent content, string queryString)
        {
            if (Authenticator == null)
                throw new Exception("Authenticator must be initialized");

            content.Headers.Add("Key", Authenticator.PublicKey);
            var signature = Authenticator.Sign(queryString);
            content.Headers.Add("Sign", signature.ToBase64String());
        }
    }
}