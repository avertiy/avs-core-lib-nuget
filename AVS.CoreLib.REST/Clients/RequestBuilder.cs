using System;
using System.Net;
using System.Text;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.REST.Helpers;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.REST.Clients
{
    public class RequestBuilder : IRequestBuilder
    {
        public IAuthenticator Authenticator { get; protected set; }
        public IWebProxy Proxy { get; set; }
        public bool AddCommandToRequestData { get; set; } = true;
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

        protected virtual void OnRequestCreating(IEndpoint endpoint, IPayload data)
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

        protected virtual void OnRequestCreated(HttpWebRequest request, IEndpoint endpoint, IPayload data)
        {
            switch (endpoint.AuthType)
            {
                case AuthType.ApiKey:
                    {
                        if (Authenticator == null)
                            throw new Exception("Authenticator must be initialized");

                        var bytes = Authenticator.GetBytes(data.ToHttpQueryString(), out string signature);
                        request.Headers["Key"] = Authenticator.PublicKey;
                        request.Headers["Sign"] = signature;
                        request.WriteBytes(bytes);
                        break;
                    }
                default:
                    if (request.Method == "POST")
                    {
                        var bytes = Encoding.ASCII.GetBytes(data.ToHttpQueryString());
                        request.WriteBytes(bytes);
                    }
                    break;
            }
        }
    }
}