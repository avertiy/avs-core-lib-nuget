using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace AVS.CoreLib.Caching
{
    public partial class XCacheManager : CacheManagerBase, IXCacheManager
    {
        /// <summary>
        /// this method aka replacement to standard extension method  _memoryCache.GetOrCreate()
        /// </summary>
        protected CachedObject<T> GetOrCreateInternal<T>(CacheKey key, Func<T> acquire, int defaultCacheTime)
        {
            if (Options.CachingEnabled == false)
                return new CachedObject<T>(acquire());

            if (TryGetValue<CachedObject<T>>(key.Key, out var cachedObj))
                return cachedObj;

            var value = acquire();
            // create CacheEntry
            CreateCacheEntry(key, value, defaultCacheTime, out var result);
            return result;
        }

        /// <summary>
        /// this method aka replacement to standard extension method  _memoryCache.GetOrCreateAsync()
        /// </summary>
        protected async Task<CachedObject<T?>> GetOrCreateInternalAsync<T>(CacheKey key, Func<Task<T?>> acquire, int defaultCacheTime)
        {
            if (Options.CachingEnabled == false)
            {
                var item = await acquire().ConfigureAwait(false);
                return new CachedObject<T?>(item);
            }

            if (TryGetValue<CachedObject<T?>>(key.Key, out var cachedObj))
                return cachedObj;

            var value = await acquire().ConfigureAwait(false);

            // create CacheEntry
            if (CreateCacheEntry(key, value, defaultCacheTime, out CachedObject<T?>? result))
                return result!;
            return new CachedObject<T?>(default);
        }

        protected bool CreateCacheEntry<T>(CacheKey key, T? value, int defaultCacheTime, out CachedObject<T>? result)
        {
            result = default;
            if (key.CacheTime == 0 || !Options.CachingEnabled)
                return false;

            if (key.CacheTime < 0 && IsSet(key.Key))
            {
                Remove(key.Key);
                return false;
            }

            // do not cache empty value
            if (value == null || value.Equals(default(T)) || value is ICollection { Count: 0 })
            {
                return false;
            }

            // create cache entry
            var options = PrepareCacheEntryOptions(key.Key, key.CacheTime ?? defaultCacheTime);

            // add expiration token that might expire cache entries by prefix
            if (key.Prefix != null)
            {
                var tokenSource = _prefixes.GetOrAdd(key.Prefix, new CancellationTokenSource());
                options.AddExpirationToken(new CancellationChangeToken(tokenSource.Token));
            }

            var wrappedValue = new CachedObject<T>(value);
            base.CreateCacheEntry(key.Key, wrappedValue, options);
            result = wrappedValue;
            return true;
        }
    }
}