using System.Net;

namespace AVS.CoreLib.Abstractions.Rest
{
    /// <summary>
    /// Build <see cref="HttpWebRequest"/> by <seealso cref="IEndpoint"/> specification
    /// if endpoint is private (require the request to be signed) uses <see cref="IAuthenticator"/> to sign the request 
    /// </summary>
    public interface IHttpRequestBuilder
    {
        IAuthenticator Authenticator { get; }
        HttpWebRequest Build(IEndpoint endpoint, IPayload data = null);
        HttpWebRequest Build(IRequest request);
    }
}