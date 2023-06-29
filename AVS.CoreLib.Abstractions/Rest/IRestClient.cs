using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IRestClient
    {
        string LastRequestedUrl { get; }

        Task<HttpWebResponse> SendRequestAsync(IRequest request, CancellationToken ct);

        [Obsolete("use SendRequestAsync(IRequest) instead")]
        Task<string> QueryAsync(IEndpoint endpoint, IPayload data = null);
        void SwitchKeys(string publicKey, string privateKey);
    }
}