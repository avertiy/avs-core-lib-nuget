using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace AVS.CoreLib.Caching
{
    /// <summary>
    /// Memory cache manager based on <see cref="IMemoryCache"/>
    /// Provides api to retrieve and refresh cached data
    /// </summary>
    public class CacheManager : CacheManagerBase, ICacheManager, IKeysBookkeeper
    {
        public ICacheKeysBookkeeper KeysBookkeeper { get; set; }
        public int DefaultCacheDuration { get; set; } = 10;
        public bool CachingEnabled { get; set; } = true;

        public CacheManager(IMemoryCache memoryCache) : base(memoryCache)
        {
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheDuration = null) //Func<T, bool> shouldCache, 
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            T value;
            if (!CachingEnabled)
            {
                value = await acquire().ConfigureAwait(false);
            }
            else
            {
                value = await GetOrCreateInternal(key, acquire, cacheDuration ?? DefaultCacheDuration);
            }

            return value;
        }

        public new bool TryGetValue<T>(string key, out T value)
        {
            if (!CachingEnabled)
            {
                value = default;
                return false;
            }

            return base.TryGetValue(key, out value);
        }

        public void Set<T>(string key, T value, int? cacheDuration = null)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null || value.Equals(default) || !CachingEnabled)
                return;

            CreateCacheEntry(key, value, cacheDuration ?? DefaultCacheDuration);
        }

        public void Refresh<T>(string key, T value, int? cacheDuration = null)
        {
            if (!CachingEnabled || key == null || value == null || value.Equals(default) || !IsSet(key))
                return;

            CreateCacheEntry(key, value, cacheDuration ?? DefaultCacheDuration);
        }

        /// <summary>
        /// this method aka replacement to standard extension method  _memoryCache.GetOrCreateAsync()
        /// </summary>
        private async Task<T> GetOrCreateInternal<T>(string key, Func<Task<T>> acquire, int cacheDuration)
        {
            if (cacheDuration > 0 && TryGetValue<T>(key, out var cachedObj))
                return cachedObj;

            var value = await acquire().ConfigureAwait(false);

            if (cacheDuration == 0)
                cacheDuration = DefaultCacheDuration;

            CreateCacheEntry(key, value, cacheDuration);
            return value;
        }

        protected override void OnCacheEntryCreated<T>(string key, T value)
        {
            KeysBookkeeper?.Add(key);
        }
        protected override void OnKeyRemoved(string key)
        {
            KeysBookkeeper?.Remove(key);
        }
    }
}