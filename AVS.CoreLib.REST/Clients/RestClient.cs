using System;
using System.Net;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.REST.Extensions;

namespace AVS.CoreLib.REST.Clients
{
    /// <summary>
    /// Utility class to send HttpWebRequest built with <see cref="IHttpRequestBuilder"/>
    /// and fetch the response
    /// </summary>
    /// <remarks>
    /// if endpoint requires Authentication, the authentication delegated to <see cref="HttpRequestBuilder"/>
    /// </remarks>
    public class RestClient : IRestClient
    {
        protected IHttpRequestBuilder HttpRequestBuilder { get; }

        public string LastRequestedUrl { get; protected set; }
        public int RequestsCounter { get; protected set; }

        public RestClient(IHttpRequestBuilder httpRequestBuilder)
        {
            HttpRequestBuilder = httpRequestBuilder;
        }

        public void SwitchKeys(string publicKey, string privateKey)
        {
            HttpRequestBuilder.SwitchKeys(publicKey, privateKey);
        }

        public Task<HttpWebResponse> SendRequestAsync(IRequest request)
        {
            var httpRequest = HttpRequestBuilder.Build(request);
            return httpRequest.FetchHttpResponseAsync();
        }

        [Obsolete("Use SendRequestAsync(IRequest request)")]
        public Task<HttpWebResponse> SendRequestAsync(IEndpoint endpoint, IPayload data = null)
        {
            var request = HttpRequestBuilder.Build(endpoint, data);
            return request.FetchHttpResponseAsync();
        }

        [Obsolete("Use SendRequestAsync(IRequest request)")]
        public Task<string> QueryAsync(IEndpoint endpoint, IPayload data = null)
        {
            var request = HttpRequestBuilder.Build(endpoint, data);
            return FetchResponse(request);
        }

        protected async Task<string> FetchResponse(HttpWebRequest request, int attempts = 2)
        {
            start:
            attempts--;
            var jsonText = await request.FetchResponseAsync();
            if (jsonText != null)
            {
                //sometimes exchange returns 422 error, but on the second attempt it is ok
                if (jsonText.Contains("The remote server returned an error: (422).") && attempts > 0)
                    goto start;
                if (jsonText.Contains("timeout") && attempts > 0)
                    goto start;
            }

            LastRequestedUrl = request.RequestUri.ToString();
            RequestsCounter++;
            return jsonText;
        }
    }
}