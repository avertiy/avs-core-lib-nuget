using System.Net.Http;

namespace AVS.CoreLib.Abstractions.Rest
{
    /// <summary>
    /// Build an <see cref="HttpRequestMessage"/> from <see cref="IRequest"/>
    /// </summary>
    public interface IRequestMessageBuilder
    {
        HttpRequestMessage Build(IRequest request);
    }

    public interface IPublicRequestMessageBuilder : IRequestMessageBuilder
    {
    }
}