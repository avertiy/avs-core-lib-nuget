#nullable enable
using System.Net.Http;
using AVS.CoreLib.Abstractions.Rest;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.REST.Clients
{
    public class PublicRestTools : RestTools, IRestTools
    {
        public PublicRestTools(IPublicRequestMessageBuilder requestMessageBuilder, IHttpClientFactory httpClientFactory, IRateLimiter rateLimiter, ILogger<PublicRestTools> logger)
            : base(requestMessageBuilder, httpClientFactory, rateLimiter, logger)
        {
        }
    }
}