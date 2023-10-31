using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IRestClient
    {
        string LastRequestedUrl { get; }
        Task<HttpWebResponse> SendRequestAsync(IRequest request, CancellationToken ct);
        void SwitchKeys(string publicKey, string privateKey);
    }
}