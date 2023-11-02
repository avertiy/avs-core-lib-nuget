#nullable enable
using System.Net.Http;
using AVS.CoreLib.Abstractions.Rest;

namespace AVS.CoreLib.REST.Clients
{
    public interface IPublicRestClient : IRestTools
    {
    }

    public class PublicRestClient : RestTools, IPublicRestClient
    {
        public PublicRestClient(IPublicRequestMessageBuilder requestMessageBuilder, IHttpClientFactory httpClientFactory, IRateLimiter rateLimiter) : base(requestMessageBuilder, httpClientFactory, rateLimiter)
        {
        }
    }
}