using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AVS.CoreLib.REST.Responses;

namespace AVS.CoreLib.REST.Extensions
{
    public static class Extensions
    {
        public static string ToStringHex(this byte[] value)
        {
            var output = string.Empty;
            for (var i = 0; i < value.Length; i++)
            {
                output += value[i].ToString("x2", CultureInfo.InvariantCulture);
            }
            return (output);
        }

        public static async Task<AggregatedResponse<T>> ToAggregatedResponseAsync<T>(this IDictionary<string, Task<Response<T>>> tasks)
        {
            await Task.WhenAll(tasks.Values);
            var aggregatedResponse = new AggregatedResponse<T>();
            foreach (var kp in tasks)
            {
                aggregatedResponse.Add(kp.Key, kp.Value.Result);
            }
            return aggregatedResponse;
        }
    }
}