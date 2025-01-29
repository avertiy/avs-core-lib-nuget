using System;
using System.Collections;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualBasic;

namespace AVS.CoreLib.Caching
{
    public abstract class CacheManagerBase : IDisposable
    {
        private bool _disposed = false;
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Raised when item is removed from cache explicitly via <see cref="Remove"/>
        /// </summary>
        public event Action<string>? ItemRemoved;
        /// <summary>
        /// Raised when item is added to cache
        /// </summary>
        public event Action<string, object>? ItemAdded;

        /// <summary>
        /// once token cancelled it causes cache entries to expire
        /// <seealso cref="Clear"/>
        /// </summary>
        protected CancellationTokenSource _clearToken = new CancellationTokenSource();

        protected CacheManagerBase(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// check whether cache entry with the given key is present
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>true if cache entry exists; otherwise false</returns>
        public bool IsSet(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }

        /// <summary>
        /// Removes from the cache entry with the given key 
        /// </summary>
        /// <param name="key">cache key</param>
        public void Remove(string key)
        {
            _memoryCache.Remove(key);
            ItemRemoved?.Invoke(key);

        }

        /// <summary>
        /// Clear all cached data
        /// </summary>
        public void Clear()
        {
            _clearToken.Cancel();
            _clearToken.Dispose();
            _clearToken = new CancellationTokenSource();
        }

        protected bool TryGetValue<T>(string key, out T? value)
        {
            if (_memoryCache.TryGetValue(key, out var obj) && obj is T val)
            {
                value = val;
                return true;
            }

            value = default;
            return false;
        }

        protected T? GetValue<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out var obj))
            {
                return (T?)obj;
            }

            return default;
        }

        protected virtual void CreateCacheEntry<T>(string key, T? item, int cacheDuration)
        {
            if (cacheDuration <= 0)
            {
                if (IsSet(key))
                    Remove(key);

                return;
            }

            if (item == null || item.Equals(default(T)) || item is ICollection { Count: 0 })
                return;

            var options = PrepareCacheEntryOptions(key, cacheDuration);
            CreateCacheEntry(key, item, options);
        }

        protected void CreateCacheEntry<T>(string key, T value, MemoryCacheEntryOptions options)
        {
            // create cache entry
            using var entry = _memoryCache.CreateEntry(key);
            entry.SetOptions(options);
            entry.Value = value;
            ItemAdded?.Invoke(key, value!);
        }



        /// <summary>
        /// Prepare cache entry options for the passed key
        /// </summary>
        protected MemoryCacheEntryOptions PrepareCacheEntryOptions(string key, int cacheTime)
        {
            //set expiration time for the passed cache key
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTime)
            };

            // add global expiration token that might expire all entries
            options.AddExpirationToken(new CancellationChangeToken(_clearToken.Token));
            return options;
        }

        #region Dispose
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
                _memoryCache.Dispose();

            _disposed = true;
        }
        #endregion
    }
}