using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.Caching
{
    /// <summary>
    /// Advanced version of <see cref="CacheManager"/>
    /// Uses <see cref="CacheKey"/> as a cache key and wrapper <see cref="CachedObject{T}"/> over cached items to give you more control over caching
    /// Supports removing cached items by prefix 
    /// </summary>
    public partial class XCacheManager : CacheManagerBase, IXCacheManager, IKeysBookkeeper
    {
        public ICacheKeysBookkeeper KeysBookkeeper { get; set; }
        public CachingOptions Options { get; }

        public XCacheManager(IMemoryCache memoryCache) : base(memoryCache)
        {
            Options = new CachingOptions() { DefaultCacheTime = 15, ShortTermCacheTime = 1 };
        }

        public XCacheManager(IMemoryCache memoryCache, IOptions<CachingOptions> options) : base(memoryCache)
        {
            Options = options.Value;
            if (Options.DefaultCacheTime == 0)
                Options.DefaultCacheTime = 15;
            if (Options.ShortTermCacheTime == 0)
                Options.ShortTermCacheTime = 1;
        }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then acquire and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key, if Cache time is not specified the DefaultCacheTime of CachingOptions will be applied</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>The cached value associated with the specified key</returns>
        public CachedObject<T> Get<T>(CacheKey key, Func<T> acquire)
        {
            var cachedObject = GetOrCreateInternal(key, acquire, Options.DefaultCacheTime);
            return cachedObject;
        }

        public new bool TryGetValue<T>(string key, out T value)
        {
            if (!Options.CachingEnabled)
            {
                value = default;
                return false;
            }

            return base.TryGetValue(key, out value);
        }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>The cached value associated with the specified key</returns>
        public Task<CachedObject<T>> GetAsync<T>(CacheKey key, Func<Task<T>> acquire)
        {
            return GetOrCreateInternalAsync(key, acquire, Options.DefaultCacheTime);
        }

        public Task<CachedObject<T>> GetShortTermAsync<T>(CacheKey key, Func<Task<T>> acquire)
        {
            return GetOrCreateInternalAsync(key, acquire, Options.ShortTermCacheTime);
        }

        public CachedObject<T> GetShortTerm<T>(CacheKey key, Func<T> acquire)
        {
            var cachedObject = GetOrCreateInternal(key, acquire, Options.ShortTermCacheTime);
            return cachedObject;
        }

        /// <summary>
        /// Set cache entry (<seealso cref="CachedObject{T}"/>) with the given key 
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="value">Value for caching</param>
        /// <param name="shortTerm">when true caching options ShortTermCacheTime is used as a default caching time</param>
        public void Set<T>(CacheKey key, T value, bool shortTerm = false)
        {
            var defaultCacheTime = shortTerm ? Options.ShortTermCacheTime : Options.DefaultCacheTime;
            CreateCacheEntry(key, value, defaultCacheTime, out _);
        }

        /// <summary>
        /// If value exists in cache update it, otherwise do nothing
        /// </summary>
        public void Refresh<T>(CacheKey key, T value, bool shortTerm = false)
        {
            if (value == null || value.Equals(default) || !IsSet(key.Key))
                return;

            var defaultCacheTime = shortTerm ? Options.ShortTermCacheTime : Options.DefaultCacheTime;
            CreateCacheEntry(key, value, defaultCacheTime, out _);
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