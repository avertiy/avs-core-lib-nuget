#nullable enable
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Rest;

namespace AVS.CoreLib.REST.Clients
{
    public interface IRestTools
    {
        string Source { get; set; }
        int RequestMinDelay { get; set; }
        DateTime RequestLastTime { get; }
        string? LastRequestedUrl { get; }
        int FetchAttempts { get; set; }
        Task<RestResponse> GetResponse(IRequest request, CancellationToken ct = default);
    }

    /// <summary>
    /// RestTools based on HttpClient which is recommended instead of HttpWebRequest in .NET Core/.NET 5+
    /// </summary>
    public class RestTools : IRestTools
    {
        private readonly IRequestMessageBuilder _requestMessageBuilder;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRateLimiter _rateLimiter;
        public string Source { get; set; } = "default";
        /// <summary>
        /// prevents from being blocked when sending multiple parallel requests
        /// API provider might set rate limits like Binance or just return access denied when detects too many requests within a small timeframe
        /// </summary>
        public int RequestMinDelay { get; set; } = 50;
        public DateTime RequestLastTime { get; private set; } = DateTime.UtcNow;
        public string? LastRequestedUrl { get; set; }
        /// <summary>
        /// sometimes api source might return 422 error, we can make a few attempts to do the request
        /// </summary>
        public int FetchAttempts { get; set; } = 2;

        public RestTools(IRequestMessageBuilder requestMessageBuilder, IHttpClientFactory httpClientFactory, IRateLimiter rateLimiter)
        {
            _requestMessageBuilder = requestMessageBuilder;
            _httpClientFactory = httpClientFactory;
            _rateLimiter = rateLimiter;
        }

        public async Task<RestResponse> GetResponse(IRequest request, CancellationToken ct = default)
        {
            // 1. rate limit
            await RateLimit(count: request.RateLimit, ct);
            // 2. prepare http request message
            using var requestMessage = GetHttpRequestMessage(request);
            // 3. send request and get response
            var response = await GetResponse(requestMessage, ct);
            // 4. adjust rate limits
            _rateLimiter.Adjust(response.StatusCode);
            return response;
        }

        protected async Task<RestResponse> GetResponse(HttpRequestMessage request, CancellationToken ct)
        {
            var client = _httpClientFactory.CreateClient(Source);
            LastRequestedUrl = request.RequestUri.ToString();

            var responseMessage = await SendAsync(client, request, FetchAttempts, ct);
            var error = await VerifyStatusCode(responseMessage);

            var response = new RestResponse(Source, responseMessage.StatusCode);
            if (error != null)
                response.Error = error;
            else
                response.Content = await responseMessage.Content.ReadAsStringAsync();

            OnResponseReady(response);
            return response;
        }

        protected async ValueTask RateLimit(int count, CancellationToken ct)
        {
            //if (_rateLimiter == null)
            //    return;
            if (!await _rateLimiter.DelayAsync(count, ct))
            {
                throw new RateLimitExceededException(Source);
            }
        }

        protected virtual async ValueTask<string?> VerifyStatusCode(HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var error = responseMessage.ReasonPhrase;
                if (string.IsNullOrEmpty(error))
                    error = $"{HttpStatusCode.TooManyRequests} - too many requests";
                return error;
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                var error = await responseMessage.Content.ReadAsStringAsync();
                return error;
            }

            return null;
        }

        protected virtual void OnResponseReady(RestResponse response)
        {
            var re = new Regex("(error|err-msg|error-message)[\"']?:[\"']?(?<error>.*?)[\"',}]", RegexOptions.IgnoreCase);
            var match = re.Match(response.Content);

            if (match.Success)
            {
                response.Error = match.Groups["error"].Value;
            }
        }

        protected virtual bool IsSuccess(HttpResponseMessage responseMessage, out string? error)
        {
            error = responseMessage.ReasonPhrase;
            if (responseMessage.StatusCode == HttpStatusCode.TooManyRequests)
            {
                if (string.IsNullOrEmpty(error))
                    error = $"{HttpStatusCode.TooManyRequests} - too many requests";

                return false;
            }

            return responseMessage.IsSuccessStatusCode;
        }

        protected HttpRequestMessage GetHttpRequestMessage(IRequest request)
        {
            return _requestMessageBuilder.Build(request);
        }
        
        protected async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage request, int attempts, CancellationToken ct)
        {
            var attempt = 0;
            EnsureRequestDelay();
            start:
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity && attempt < attempts)
            {
                attempt++;
                goto start;
            }

            return response;
        }

        protected void EnsureRequestDelay()
        {
            if (RequestMinDelay <= 0)
                return;

            var ts = DateTime.UtcNow - RequestLastTime;
            if (ts.TotalMilliseconds < RequestMinDelay)
            {
                var timeout = RequestMinDelay - Convert.ToInt32(ts.TotalMilliseconds);
                Thread.Sleep(timeout);
            }

            RequestLastTime = DateTime.UtcNow;
        }
    }

}