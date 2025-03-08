using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AVS.CoreLib.Collections;
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

        public CacheManager(IMemoryCache memoryCache, int fixedListKeysCapacity = 100) : base(memoryCache, fixedListKeysCapacity)
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
            if (!CachingEnabled || string.IsNullOrEmpty(key) || value == null || value.Equals(default) || !IsSet(key))
                return;

            CreateCacheEntry(key, value, cacheDuration ?? DefaultCacheDuration);
        }

        public async Task<CacheResult<T>> GetOrCreate<T>(string key, Func<Task<T>> acquire, int? cacheDuration = null)
        {
            if (TryGetValue(key, out T? val) && val !=null)
                return new CacheResult<T>(val, true);

            var value = await acquire().ConfigureAwait(false);

            var durationInMinutes = cacheDuration ?? DefaultCacheDuration;
            if (durationInMinutes > 0)
                CreateCacheEntry(key, value, durationInMinutes);

            return value;
        }
    }
}