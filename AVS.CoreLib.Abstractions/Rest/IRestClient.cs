using System;
using System.Net;
using System.Threading.Tasks;

namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IRestClient
    {
        string LastRequestedUrl { get; }

        Task<HttpWebResponse> SendRequestAsync(IRequest request);

        //[Obsolete("use SendRequestAsync instead")]
        Task<string> QueryAsync(IEndpoint endpoint, IPayload data = null);

        //[Obsolete("use SendRequestAsync instead")]
        Task<HttpWebResponse> SendRequestAsync(IEndpoint endpoint, IPayload data = null);
        void SwitchKeys(string publicKey, string privateKey);
    }
}