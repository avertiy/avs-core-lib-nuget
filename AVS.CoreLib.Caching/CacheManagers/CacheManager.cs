using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace AVS.CoreLib.Caching
{
    /// <summary>
    /// Memory cache manager based on <see cref="IMemoryCache"/>
    /// Provides api to retrieve and refresh cached data
    /// </summary>
    public class CacheManager : CacheManagerBase, ICacheManager
    {
        public int DefaultCacheDuration { get; set; } = 10;
        public bool CachingEnabled { get; set; } = true;

        public CacheManager(IMemoryCache memoryCache) : base(memoryCache)
        {
        }

        public new bool TryGetValue<T>(string key, out T? value)
        {
            if (!CachingEnabled)
            {
                value = default;
                return false;
            }

            return base.TryGetValue(key, out value);
        }

        public void Set<T>(string key, T? item, int? cacheDuration = null)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (item == null || item.Equals(default) || !CachingEnabled)
                return;

            CreateCacheEntry(key, item, cacheDuration ?? DefaultCacheDuration);
        }

        public void Refresh<T>(string key, T value, int? cacheDuration = null)
        {
            if (!CachingEnabled || key == null || value == null || value.Equals(default) || !IsSet(key))
                return;

            CreateCacheEntry(key, value, cacheDuration ?? DefaultCacheDuration);
        }

        public async Task<T?> GetOrCreate<T>(string key, Func<Task<T?>> acquire, int? cacheDuration = null)
        {
            if (CachingEnabled && TryGetValue(key, out T? cachedObj))
                return cachedObj;

            var value = await acquire().ConfigureAwait(false);

            var time = cacheDuration ?? DefaultCacheDuration;
            if (time > 0)
                CreateCacheEntry(key, value, time);
            return value;
        }
    }
}