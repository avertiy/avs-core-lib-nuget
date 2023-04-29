using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.REST.Types;

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

        protected virtual async Task<JsonResult> Query(IEndpoint endpoint, IPayload data)
        {
            var text = await Client.QueryAsync(endpoint, data).ConfigureAwait(false);
            return new JsonResult() { JsonText = text, Source = Name };
        }

        protected virtual async Task<JsonResult> SendRequest(IRequest request, CancellationToken ct = default)
        {
            var response = await Client.SendRequestAsync(request, ct).ConfigureAwait(false);
            var result = JsonResult.FromResponse(response);
            return result;
        }

        protected async Task<JsonResult> Query(string url, string method = "GET")
        {
            var text = await Client.QueryAsync(new ApiEndpoint(url, "", method)).ConfigureAwait(false);
            return new JsonResult() { JsonText = text, Source = Name };
        }
    }
}