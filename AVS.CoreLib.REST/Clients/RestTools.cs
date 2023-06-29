using System;
using System.Net;
using System.Net.Http;
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

        public string LastRequestedUrl { get; set; }

        /// <summary>
        /// sometimes api source might return 422 error, we can make a few attempts to do the request
        /// </summary>
        public int FetchAttempts { get; set; } = 2;
        protected RestTools(IRequestMessageBuilder requestMessageBuilder, IHttpClientFactory httpClientFactory)
        {
            _requestMessageBuilder = requestMessageBuilder;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<JsonResult> SendRequestAsync(IRequest request, CancellationToken ct = default)
        {
            EnsureRequestDelay();
            using (var requestMessage = _requestMessageBuilder.Build(request))
            {
                var result = await Fetch(requestMessage, ct);
                return result;
            }
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

        private async Task<JsonResult> Fetch(HttpRequestMessage request, CancellationToken ct)
        {
            var client = _httpClientFactory.CreateClient(Source);
            LastRequestedUrl = request.RequestUri.ToString();
            var attempt = 0;
            start:

            try
            {
                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct)
                    .ConfigureAwait(false);

                //StatusCode == 422
                if (response.StatusCode == HttpStatusCode.UnprocessableEntity && attempt < FetchAttempts)
                {
                    attempt++;
                    goto start;
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = response.IsSuccessStatusCode ?
                    new JsonResult() { Source = Source, JsonText = content } :
                    new JsonResult() { Source = Source, Error = content };

                response.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                return new JsonResult() { Source = Source, Error = ex.ToString() };
            }
        }
    }
}