using System;
using System.Linq;
using System.Net;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.REST.Extensions;
using AVS.CoreLib.REST.Helpers;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.REST.Clients
{
    /// <summary>
    /// build <see cref="HttpWebRequest"/> for public/private endpoints by <see cref="IEndpoint"/> specification
    /// </summary>
    public class HttpRequestBuilder : IHttpRequestBuilder
    {
        public IAuthenticator Authenticator { get; protected set; }

        /// <summary>
        /// Gets or sets the proxy information of the <see cref="HttpWebRequest"/>
        /// </summary>
        public IWebProxy Proxy { get; set; }
        public bool AddCommandToRequestData { get; set; }
        public bool UseTonce { get; set; }

        public HttpRequestBuilder(IAuthenticator authenticator)
        {
            Authenticator = authenticator;
        }

        public virtual HttpWebRequest Build(IRequest request)
        {
            try
            {
                OnRequestCreating(request);

                var url = request.GetFullUrl();
                var httpRequest = WebRequestHelper.ConstructHttpWebRequest(request.Method, url, Proxy);

                OnRequestCreated(httpRequest, request);
                return httpRequest;
            }
            catch (Exception ex)
            {
                throw new Exception("Constructing HttpWebRequest failed", ex);
            }
        }

        protected virtual void OnRequestCreating(IRequest request)
        {
        }

        /// <summary>
        /// private endpoints require request headers to be provided
        /// by default sets Headers["Key"] = public_key; Headers["Sign"] = signature
        /// </summary>
        protected virtual void OnRequestCreated(HttpWebRequest httpRequest, IRequest request)
        {
            switch (request.AuthType)
            {
                case AuthType.ApiKey:
                {
                    if (Authenticator == null)
                        throw new Exception("Authenticator must be initialized");

                    var signature = Authenticator.Sign(request.Data.ToHttpQueryString(), out var bytes);
                    httpRequest.Headers["Key"] = Authenticator.PublicKey;
                    httpRequest.Headers["Sign"] = signature.ToBase64String();
                    httpRequest.WriteBytes(bytes);
                    break;
                }
                default:
                    if (httpRequest.Method != "GET")
                    {
                        var bytes = Authenticator.Encoding.GetBytes(request.Data.ToHttpQueryString());
                        httpRequest.WriteBytes(bytes);
                    }
                    break;
            }
        }


        public virtual HttpWebRequest Build(IEndpoint endpoint, IPayload data = null)
        {
            try
            {
                OnRequestCreating(endpoint, data);

                var url = GetUrl(endpoint, data);
                var request = WebRequestHelper.ConstructHttpWebRequest(endpoint.Method, url, Proxy);

                OnRequestCreated(request, endpoint, data);
                return request;
            }
            catch (Exception ex)
            {
                throw new Exception("Constructing HttpWebRequest failed", ex);
            }
        }

        protected string GetUrl(IEndpoint endpoint, IPayload data)
        {
            var url = endpoint.Url + data?.RelativeUrl;
            if (endpoint.Method == "GET" && data != null)
                url = UrlHelper.Combine(url, data.ToHttpQueryString());

            return url;
        }

        /// <summary>
        /// use OnRequestCreating to add something to each payload
        /// for example private endpoints require tonce/nonce to be provided in payload  
        /// </summary>
        protected virtual void OnRequestCreating(IEndpoint endpoint, IPayload data)
        {
            if (endpoint.AuthType == AuthType.ApiKey)
            {
                // some endpoints require command to be added into payload
                if (AddCommandToRequestData)
                    data.Add("command", endpoint.Command);

                // some endpoints use tonce, which is slight ly different from nonce
                if (UseTonce)
                    data.Add("tonce", NonceHelper.GetTonce());
                else
                    data.Add("nonce", NonceHelper.GetNonce());
            }
        }

        /// <summary>
        /// private endpoints require request headers to be provided
        /// by default sets Headers["Key"] = public_key; Headers["Sign"] = signature
        /// </summary>
        protected virtual void OnRequestCreated(HttpWebRequest httpRequest, IEndpoint endpoint, IPayload data)
        {
            switch (endpoint.AuthType)
            {
                case AuthType.ApiKey:
                    {
                        if (Authenticator == null)
                            throw new Exception("Authenticator must be initialized");

                        var signature = Authenticator.Sign(data.ToHttpQueryString(), out var bytes);
                        httpRequest.Headers["Key"] = Authenticator.PublicKey;
                        httpRequest.Headers["Sign"] = signature.ToBase64String();
                        httpRequest.WriteBytes(bytes);
                        break;
                    }
                default:
                    if (httpRequest.Method != "GET")
                    {
                        var bytes = Authenticator.Encoding.GetBytes(data.ToHttpQueryString());
                        httpRequest.WriteBytes(bytes);
                    }
                    break;
            }
        }
    }
}