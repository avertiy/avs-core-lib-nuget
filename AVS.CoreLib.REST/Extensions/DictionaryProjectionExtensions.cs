#nullable enable
using AVS.CoreLib.REST.Responses;

namespace AVS.CoreLib.REST.Extensions
{
    public static class DictionaryProjectionExtensions
    {
        public static BoolResponse ToBoolResponse(this RestResponse result)
        {
            if (!result.TryDeserialize(out BoolResponse response, out var error))
                response = new BoolResponse() { Error = error };

            response.Source = result.Source;
            return response;
        }
    }
}