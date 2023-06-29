using System.Net.Http;

namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IRequestMessageBuilder
    {
        IAuthenticator Authenticator { get; }
        HttpRequestMessage Build(IRequest input);
    }
}