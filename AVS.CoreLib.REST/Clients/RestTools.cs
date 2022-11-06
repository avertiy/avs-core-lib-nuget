using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.REST.Extensions;
using AVS.CoreLib.REST.Types;

namespace AVS.CoreLib.REST.Clients
{
    /// <summary>
    /// RestTools are based on HttpClient
    /// It is to replace a RestClient
    /// </summary>
    public class RestTools
    {
        private readonly IRequestMessageBuilder _requestMessageBuilder;
        protected string Source { get; set; }
        public string LastRequestedUrl;

        /// <summary>
        /// sometimes api source might return 422 error, we can make a few attempts to do the request
        /// </summary>
        public int FetchAttempts { get; set; } = 2;
        protected RestTools(IRequestMessageBuilder requestMessageBuilder)
        {
            _requestMessageBuilder = requestMessageBuilder;
        }

        public Task<JsonResult> QueryAsync(string url, string method = "GET", IPayload data = null, CancellationToken cancellationToken = default)
        {
            return QueryAsync(new ApiEndpoint(url, "", method), data, cancellationToken);
        }

        public async Task<JsonResult> QueryAsync(ApiEndpoint endpoint, IPayload data = null, CancellationToken cancellationToken = default)
        {
            var text = await SendAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
            return new JsonResult() { JsonText = text, Source = Source ?? endpoint.Url };
        }

        protected Task<string> SendAsync(ApiEndpoint endpoint, IPayload data, CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            using (var request = _requestMessageBuilder.Build(endpoint, data))
            {
                LastRequestedUrl = request.RequestUri.ToString();
                return client.ExecuteStringAsync(request, cancellationToken, FetchAttempts);
            }
        }
    }
}