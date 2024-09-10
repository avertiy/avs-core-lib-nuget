using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Rest;

namespace AVS.CoreLib.REST.Clients
{
    public abstract class ApiClientBase
    {
        public abstract string Name { get; }
        protected IRestClient Client { get; }
        public string LastRequestedUrl => Client.LastRequestedUrl;

        protected ApiClientBase(IRestClient client)
        {
            Client = client;
        }

        public void SwitchKeys(string publicKey, string privateKey)
        {
            Client.SwitchKeys(publicKey, privateKey);
        }

        protected virtual async Task<RestResponse> SendRequest(IRequest request, CancellationToken ct = default)
        {
            var response = await Client.SendRequestAsync(request, ct).ConfigureAwait(false);
            var result = RestResponse.FromResponse(response, Name, request);
            return result;
        }
    }
}