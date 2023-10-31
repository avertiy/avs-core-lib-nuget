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
    /// <summary>
    /// RestTools based on HttpClient which is recommended instead of HttpWebRequest in .NET Core/.NET 5+
    /// </summary>
    public class RestTools
    {
        private readonly IRequestMessageBuilder _requestMessageBuilder;
        private readonly IHttpClientFactory _httpClientFactory;
        public string Source { get; set; }
        /// <summary>
        /// prevents from being blocked when sending multiple parallel requests
        /// API provider might set rate limits like Binance or just return access denied when detects too many requests within a small timeframe
        /// </summary>
        public int RequestMinDelay { get; set; } = 50;
        public DateTime LastRequestTime { get; private set; } = DateTime.UtcNow;
        public string? LastRequestedUrl { get; set; }
        /// <summary>
        /// sometimes api source might return 422 error, we can make a few attempts to do the request
        /// </summary>
        public int FetchAttempts { get; set; } = 2;

        public RestTools(IRequestMessageBuilder requestMessageBuilder, IHttpClientFactory httpClientFactory, string source = "default")
        {
            _requestMessageBuilder = requestMessageBuilder;
            _httpClientFactory = httpClientFactory;
            Source = source;
        }

        public async Task<RestResponse> GetResponse(IRequest request, CancellationToken ct = default)
        {
            // 1. prepare http request message
            using var requestMessage = GetHttpRequestMessage(request);
            // 2. send request and get response
            var response = await GetResponse(requestMessage, ct);
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

            var ts = DateTime.UtcNow - LastRequestTime;
            if (ts.TotalMilliseconds < RequestMinDelay)
            {
                var timeout = RequestMinDelay - Convert.ToInt32(ts.TotalMilliseconds);
                Thread.Sleep(timeout);
            }

            LastRequestTime = DateTime.UtcNow;
        }
    }

    public static class RestToolsExtensions
    {
        //public static async Task<JsonResult> FetchJson(this RestTools tools, IRequest request, CancellationToken ct = default)
        //{
        //    var response = await tools.GetResponse(request, ct);
        //    var res = new JsonResult(response.Source, response.Content, response.Error);
        //}
    }

    //[Obsolete("Use Fetch json")]
    //public virtual async Task<JsonResult> SendRequestAsync(IRequest request, CancellationToken ct = default)
    //{
    //    EnsureRequestDelay();
    //    using (var requestMessage = _requestMessageBuilder.Build(request))
    //    {
    //        var result = await Fetch(requestMessage, ct);
    //        return result;
    //    }
    //}   
  
    //protected virtual async Task<JsonResult> Fetch(HttpRequestMessage request, CancellationToken ct)
    //{
    //    var client = _httpClientFactory.CreateClient(Source);
    //    LastRequestedUrl = request.RequestUri.ToString();
    //    try
    //    {
    //        var response = await SendAsync(client, request, FetchAttempts, ct);

    //        if (response.StatusCode == HttpStatusCode.TooManyRequests)
    //        {
    //            var error = await response.Content.ReadAsStringAsync();
    //            if (string.IsNullOrEmpty(error))
    //                error = response.ReasonPhrase;
    //            throw new TooManyRequestsApiException(error);
    //        }

    //        JsonResult result;

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            var error = await response.Content.ReadAsStringAsync();
    //            if (string.IsNullOrEmpty(error))
    //                error = response.ReasonPhrase;
    //            result = JsonResult.Failed(Source, error);
    //        }
    //        else
    //        {
    //            var content = await response.Content.ReadAsStringAsync();
    //            result = JsonResult.Success(Source, content);
    //        }

    //        response.Dispose();
    //        return result;
    //    }
    //    catch (Exception ex)
    //    {
    //        return JsonResult.Failed(Source, ex.ToString());
    //    }
    //}
}