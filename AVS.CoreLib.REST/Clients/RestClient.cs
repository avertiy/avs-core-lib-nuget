﻿using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.REST.Extensions;

namespace AVS.CoreLib.REST.Clients
{
    /// <summary>
    /// RestClient is based on HttpWebRequest
    /// </summary>
    public class RestClient : IRestClient
    {
        protected IRequestBuilder RequestBuilder { get; }
        public string Name { get; protected set; }
        public string LastRequestedUrl { get; protected set; }
        public int RequestsCounter { get; protected set; }

        public RestClient(string name, string publicKey, string privateKey)
        {
            Name = name;
            RequestBuilder = new RequestBuilder(new HMACSHA512Authenticator(publicKey, privateKey));
        }

        public RestClient(string name, IRequestBuilder requestBuilder)
        {
            Name = name;
            RequestBuilder = requestBuilder;
        }

        public Task<string> QueryAsync(IEndpoint endpoint, IPayload data = null)
        {
            var request = RequestBuilder.Build(endpoint, data);
            return FetchResponse(request);
        }

        public void SwitchKeys(string publicKey, string privateKey)
        {
            RequestBuilder.Authenticator.SwitchKeys(publicKey, privateKey);
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

    public static class HttpClientExtensions
    {
        public static async Task<string> ExecuteStringAsync(this HttpClient client, HttpRequestMessage request,
            CancellationToken cancellationToken, int attempts = 2)
        {
            start:
            attempts--;
            using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                var text = await response.Content.ReadAsStringAsync();
                if (text != null && text.Contains("The remote server returned an error: (422).") && attempts > 0)
                    goto start;
                return text;
            }
        }
    }

}