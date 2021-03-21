using System.Net;

namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IRequestBuilder
    {
        IAuthenticator Authenticator { get; }
        HttpWebRequest Build(IEndpoint endpoint, IPayload data = null);
    }
}