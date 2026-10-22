using System.Net.Http;

namespace AVS.CoreLib.Abstractions.Rest
{
    /// <summary>
    /// Build an <see cref="HttpRequestMessage"/> from <see cref="IRequest"/>
    /// </summary>
    public interface IRequestMessageBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        HttpRequestMessage Build(IRequest request);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IPublicRequestMessageBuilder : IRequestMessageBuilder
    {
    }
}