using System;
using System.Net;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.REST.Extensions;
using AVS.CoreLib.REST.Helpers;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.REST.Clients
{
    /// <summary>
    /// build <see cref="HttpWebRequest"/> for public/private endpoints by <see cref="IEndpoint"/> specification
    /// </summary>
    public class RequestBuilder : IRequestBuilder
    {
        public IAuthenticator Authenticator { get; protected set; }

        /// <summary>
        /// Gets or sets the proxy information of the <see cref="HttpWebRequest"/>
        /// </summary>
        public IWebProxy Proxy { get; set; }
        public bool AddCommandToRequestData { get; set; }
        public bool UseTonce { get; set; }

        public RequestBuilder(IAuthenticator authenticator)
        {
            Authenticator = authenticator;
        }

        public HttpWebRequest Build(IEndpoint endpoint, IPayload data = null)
        {
            try
            {
                OnRequestCreating(endpoint, data);
                var url = endpoint.Url + data?.RelativeUrl;
                if (endpoint.Method == "GET" && data != null)
                    url = UrlHelper.Combine(url, data.ToHttpQueryString());

                var request = WebRequestHelper.ConstructHttpWebRequest(endpoint.Method, url);
                if (Proxy != null)
                    request.Proxy = Proxy;

                OnRequestCreated(request, endpoint, data);
                return request;
            }
            catch (Exception ex)
            {
                throw new Exception("CreateRequest failed", ex);
            }
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
        protected virtual void OnRequestCreated(HttpWebRequest request, IEndpoint endpoint, IPayload data)
        {
            switch (endpoint.AuthType)
            {
                case AuthType.ApiKey:
                    {
                        if (Authenticator == null)
                            throw new Exception("Authenticator must be initialized");

                        var signature = Authenticator.Sign(data.ToHttpQueryString(), out var bytes);
                        request.Headers["Key"] = Authenticator.PublicKey;
                        request.Headers["Sign"] = signature.ToBase64String();
                        request.WriteBytes(bytes);
                        break;
                    }
                default:
                    if (request.Method != "GET")
                    {
                        var bytes = Authenticator.Encoding.GetBytes(data.ToHttpQueryString());
                        request.WriteBytes(bytes);
                    }
                    break;
            }
        }
    }
}