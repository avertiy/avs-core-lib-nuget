using System;
using System.Net;

namespace AVS.CoreLib.Abstractions.Rest
{
    /// <summary>
    /// Build <see cref="HttpWebRequest"/> by <seealso cref="IEndpoint"/> specification
    /// if endpoint is private (require the request to be signed) uses <see cref="IAuthenticator"/> to sign the request 
    /// </summary>
    public interface IHttpRequestBuilder
    {
        HttpWebRequest Build(IRequest request);
        void SwitchKeys(string publicKey, string privateKey);

        [Obsolete("Use Build(IRequest request)")]
        HttpWebRequest Build(IEndpoint endpoint, IPayload data = null);
    }
}