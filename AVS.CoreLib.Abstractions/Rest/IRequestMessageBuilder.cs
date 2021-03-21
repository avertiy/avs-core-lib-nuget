using System.Net.Http;

namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IRequestMessageBuilder
    {
        HttpRequestMessage Build(IEndpoint endpoint, IPayload data);
    }
}