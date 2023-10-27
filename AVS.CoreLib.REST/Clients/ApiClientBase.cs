using System;
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

        protected virtual async Task<JsonResult> SendRequest(IRequest request, CancellationToken ct = default)
        {
            var response = await Client.SendRequestAsync(request, ct).ConfigureAwait(false);
            var result = JsonResult.FromResponse(response, Name);
            return result;
        }        
    }
}