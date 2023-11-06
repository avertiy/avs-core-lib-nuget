using System;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Responses;

namespace AVS.CoreLib.Caching
{
    public static class CacheManagerExtensions
    {
        /// <summary>       
        /// Fetch data if response is success creates a cache entry
        /// when key is null or cache duration is 0 - does only fetch
        /// </summary>
        public static async Task<CacheResult<TResponse>> GetOrFetch<TResponse>(this ICacheManager cacheManager, string? key, Func<Task<TResponse>> fetch, int cacheDuration)
            where TResponse : IResponse
        {
            if (key !=null && cacheManager.TryGetValue(key, out TResponse? response))
                return new CacheResult<TResponse>(response!, true);

            response = await fetch();

            if (key !=null && response.Success && cacheDuration > 0)
                cacheManager.Set(key, response, cacheDuration);

            return response!;
        }        
    }
}