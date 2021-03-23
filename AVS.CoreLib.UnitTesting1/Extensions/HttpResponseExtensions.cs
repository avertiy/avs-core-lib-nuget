using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AVS.CoreLib.UnitTesting.Extensions
{
    public static class HttpResponseExtensions
    {
        public static async Task<T> ReadBody<T>(this HttpResponseMessage response)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
