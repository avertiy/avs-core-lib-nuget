using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace AVS.CoreLib.Caching
{
    /// <summary>
    /// Represents a memory cache manager 
    /// </summary>
    public class MemoryCacheManager : ICacheManager
    {
        #region Fields

        // Flag: Has Dispose already been called?
        private bool _disposed;
        private readonly CachingOptions _options;
        private readonly IMemoryCache _memoryCache;

        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _prefixes = new ConcurrentDictionary<string, CancellationTokenSource>();
        private static CancellationTokenSource _clearToken = new CancellationTokenSource();

        #endregion

        #region Ctor

        public MemoryCacheManager(IMemoryCache memoryCache, IOptions<CachingOptions> options)
        {
            _memoryCache = memoryCache;
            _options = options.Value;
            if (_options.DefaultCacheTime == 0)
                _options.DefaultCacheTime = 15;
            if (_options.ShortTermCacheTime == 0)
                _options.ShortTermCacheTime = 1;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare cache entry options for the passed key
        /// </summary>
        private MemoryCacheEntryOptions PrepareEntryOptions(CacheKey key, int defaultCacheTime)
        {
            //set expiration time for the passed cache key
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(key.CacheTime?? defaultCacheTime)
            };

            //add tokens to clear cache entries
            if (key.Prefix != null)
            {
                var tokenSource = _prefixes.GetOrAdd(key.Prefix, new CancellationTokenSource());
                options.AddExpirationToken(new CancellationChangeToken(tokenSource.Token));
            }

            options.AddExpirationToken(new CancellationChangeToken(_clearToken.Token));

            return options;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key, if Cache time is not specified the DefaultCacheTime of CachingOptions will be applied</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>The cached value associated with the specified key</returns>
        public CachedObject<T> Get<T>(CacheKey key, Func<T> acquire)
        {
            CachedObject<T> result;
            if (key.CacheTime <= 0 || _options.CachingEnabled == false)
            {
                result = new CachedObject<T>(acquire());
                Refresh(key, result, false);
            }
            else
            {
                result = _memoryCache.GetOrCreate(key.Key, entry =>
                {
                    entry.SetOptions(PrepareEntryOptions(key, _options.DefaultCacheTime));
                    return new CachedObject<T>(acquire());
                });
            }

            //do not cache null value
            if (result.IsNullOrEmpty)
                Remove(key);

            return result;
        }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>The cached value associated with the specified key</returns>
        public async Task<CachedObject<T>> GetAsync<T>(CacheKey key, Func<Task<T>> acquire)
        {
            CachedObject<T> result;
            if (key.CacheTime <= 0 || _options.CachingEnabled == false)
            {
                result = new CachedObject<T>(await acquire());
                Refresh(key, result, false);
            }
            else
            {
                result = await _memoryCache.GetOrCreateAsync(key.Key, async entry =>
                {
                    entry.SetOptions(PrepareEntryOptions(key, _options.DefaultCacheTime));
                    return new CachedObject<T>(await acquire());
                });
            }

            //do not cache null value
            if (result.IsNullOrEmpty)
                Remove(key);

            return result;
        }

        public async Task<CachedObject<T>> GetShortTermAsync<T>(CacheKey key, Func<Task<T>> acquire)
        {
            CachedObject<T> result;
            if (key.CacheTime <= 0 || _options.CachingEnabled == false)
            {
                result = new CachedObject<T>(await acquire());
                Refresh(key, result, true);
            }
            else
            {
                result = await _memoryCache.GetOrCreateAsync(key.Key, async entry =>
                {
                    entry.SetOptions(PrepareEntryOptions(key, _options.ShortTermCacheTime));
                    var value = await acquire();
                    return new CachedObject<T>(value);
                });
            }

            //do not cache null value
            if (result.IsNullOrEmpty)
                Remove(key);

            return result;
        }

        public CachedObject<T> GetShortTerm<T>(CacheKey key, Func<T> acquire)
        {
            CachedObject<T> result;
            if (key.CacheTime <= 0 || _options.CachingEnabled == false)
            {
                result = new CachedObject<T>(acquire());
                Refresh(key, result, true);
            }
            else
            {
                result = _memoryCache.GetOrCreate(key.Key, entry =>
                {
                    entry.SetOptions(PrepareEntryOptions(key, _options.ShortTermCacheTime));
                    var value = acquire();
                    return new CachedObject<T>(value);
                });
            }
            
            //do not cache null value
            if (result.IsNullOrEmpty)
                Remove(key);

            return result;
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        public void Remove(CacheKey key)
        {
            _memoryCache.Remove(key.Key);
        }

        /// <summary>
        /// Adds the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        /// <param name="shortTerm">when true caching options ShortTermCacheTime is used as a default caching time</param>
        public void Set(CacheKey key, object data, bool shortTerm = false)
        {
            if (key.CacheTime <= 0 || data == null)
                return;
            _memoryCache.Set(key.Key, data, TimeSpan.Zero);
            _memoryCache.Set(key.Key, data, PrepareEntryOptions(key, shortTerm? _options.ShortTermCacheTime : _options.DefaultCacheTime));
        }

        /// <summary>
        /// If value exists in cache update it, otherwise do nothing
        /// </summary>
        public void Refresh(CacheKey key, object data, bool shortTerm = false)
        {
            if (IsSet(key))
            {
                _memoryCache.Set(key.Key, data, PrepareEntryOptions(key, shortTerm ? _options.ShortTermCacheTime : _options.DefaultCacheTime));
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>True if item already is in cache; otherwise false</returns>
        public bool IsSet(CacheKey key)
        {
            return _memoryCache.TryGetValue(key.Key, out _);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        /// <summary>
        /// Removes items by key prefix
        /// </summary>
        /// <param name="prefix">String key prefix</param>
        public void RemoveByPrefix(string prefix)
        {
            _prefixes.TryRemove(prefix, out var tokenSource);
            tokenSource?.Cancel();
            tokenSource?.Dispose();
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public void Clear()
        {
            _clearToken.Cancel();
            _clearToken.Dispose();

            _clearToken = new CancellationTokenSource();
        }

        /// <summary>
        /// Dispose cache manager
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _memoryCache.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}