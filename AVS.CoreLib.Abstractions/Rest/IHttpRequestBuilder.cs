using System.Net;

namespace AVS.CoreLib.Abstractions.Rest
{
    /// <summary>
    /// Build <see cref="HttpWebRequest"/> by <seealso cref="IEndpoint"/> specification
    /// if endpoint is private (require the request to be signed) uses <see cref="IAuthenticator"/> to sign the request 
    /// </summary>
    public interface IHttpRequestBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        HttpWebRequest Build(IRequest request);
        /// <summary>
        /// 
        /// </summary>
        void SwitchKeys(string publicKey, string privateKey, string? password = null);
    }
}