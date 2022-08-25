using System.Threading.Tasks;

namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IRestClient
    {
        string LastRequestedUrl { get; }
        Task<string> QueryAsync(IEndpoint endpoint, IPayload data = null);
        void SwitchKeys(string publicKey, string privateKey);
    }
}