using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Json.Extensions;

namespace AVS.CoreLib.REST.Extensions
{
    public static class PayloadExtensions
    {
        public static string ToJson(this IPayload payload)
        {
            return payload.Items.ToJson();
        }
    }
}