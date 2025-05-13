using System;
using System.Net;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.Extensions.Web;
using AVS.CoreLib.REST.Extensions;
using AVS.CoreLib.REST.Helpers;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.REST.RequestBuilders
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
        public bool OrderQueryStringParameters { get; set; }

        public HttpRequestBuilder(IAuthenticator authenticator)
        {
            Authenticator = authenticator;
        }

        public HttpWebRequest Build(IRequest request)
        {
            try
            {
                return CreateHttpRequest(request);
            }
            catch (Exception ex)
            {
                throw new Exception("Constructing HttpWebRequest failed", ex);
            }
        }

        public void SwitchKeys(string publicKey, string privateKey)
        {
            Authenticator.SetKeys(publicKey, privateKey);
        }

        protected virtual HttpWebRequest CreateHttpRequest(IRequest request)
        {
            OnRequestCreating(request);
            var url = request.GetFullUrl(OrderQueryStringParameters);
            var httpRequest = WebRequestHelper.ConstructHttpWebRequest(request.HttpMethod, url, Proxy);
            OnRequestCreated(httpRequest, request);
            return httpRequest;
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

                        var qs = request.Data?.ToHttpQueryString() ?? string.Empty;
                        var signature = Authenticator.Sign(qs, out var bytes);
                        httpRequest.Headers["Key"] = Authenticator.PublicKey;
                        httpRequest.Headers["Sign"] = signature.ToBase64String();
                        //work around for stupid .net protocol violation check
                        httpRequest.Method = "POST";
                        httpRequest.WriteBytes(bytes);
                        httpRequest.Method = request.HttpMethod;
                        break;
                    }
                default:
                    if (httpRequest.Method != "GET")
                    {
                        var qs = request.Data?.ToHttpQueryString() ?? string.Empty;
                        var bytes = Authenticator.Encoding.GetBytes(qs);
                        httpRequest.WriteBytes(bytes);
                    }
                    break;
            }
        }

        /*
        [Obsolete("Use Build(IRequest request)")]
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
        }*/

        /// <summary>
        /// use OnRequestCreating to add something to each payload
        /// for example private endpoints require tonce/nonce to be provided in payload  
        /// </summary>
        [Obsolete("Use Build(IRequest request)")]
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
                    data.Add("nonce", NonceHelper.GetNonce().ToString());
            }
        }

        /// <summary>
        /// private endpoints require request headers to be provided
        /// by default sets Headers["Key"] = public_key; Headers["Sign"] = signature
        /// </summary>
        [Obsolete("Use Build(IRequest request)")]
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
                        //work around for stupid .net protocol violation check
                        httpRequest.Method = "POST";
                        httpRequest.WriteBytes(bytes);
                        httpRequest.Method = endpoint.Method;
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

    public class HttpRequestBuilder<TAuthenticator> : IHttpRequestBuilder
        where TAuthenticator : IAuthenticator
    {
        public TAuthenticator Authenticator { get; }

        /// <summary>
        /// Gets or sets the proxy information of the <see cref="HttpWebRequest"/>
        /// </summary>
        public IWebProxy Proxy { get; set; }
        public bool OrderQueryStringParameters { get; set; }
        public string ContentType { get; set; } = "application/x-www-form-urlencoded";

        public HttpRequestBuilder(TAuthenticator authenticator)
        {
            Authenticator = authenticator;
        }

        public HttpWebRequest Build(IRequest input)
        {
            try
            {
                return CreateRequest(input);
            }
            catch (Exception ex)
            {
                throw new Exception($"Constructing HttpWebRequest failed [input: {input}]", ex);
            }

        }

        public void SwitchKeys(string publicKey, string privateKey)
        {
            Authenticator.SetKeys(publicKey, privateKey);
        }

        public HttpWebRequest Build(IEndpoint endpoint, IPayload data = null)
        {
            throw new NotImplementedException();
        }

        protected virtual HttpWebRequest CreateRequest(IRequest input)
        {
            OnRequestCreating(input);
            var httpRequest = CreateHttpWebRequest(input, OrderQueryStringParameters, Proxy, ContentType);
            OnRequestCreated(httpRequest, input);
            return httpRequest;
        }

        protected virtual void OnRequestCreating(IRequest input)
        {
        }

        /// <summary>
        /// write bytes (payload) if method is not GET i.e. POST/PUT etc.  
        /// </summary>
        protected virtual void OnRequestCreated(HttpWebRequest httpRequest, IRequest input)
        {
            switch (httpRequest.Method)
            {
                case "GET":
                    {
                        // do nothing, payload is already in query string 
                        break;
                    }
                default:
                    {
                        var bytes = Authenticator.Encoding.GetBytes(input.Data.ToHttpQueryString());
                        httpRequest.WriteBytes(bytes);
                        break;
                    }
            }
        }

        protected static HttpWebRequest CreateHttpWebRequest(IRequest input, bool orderQueryStringParameters, IWebProxy proxy, string contentType)
        {
            var url = input.GetFullUrl(orderQueryStringParameters);
            return WebRequestHelper.ConstructHttpWebRequest(input.HttpMethod, url, proxy, contentType);
        }
    }
}