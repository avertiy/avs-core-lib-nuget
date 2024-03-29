﻿using System;
using System.Threading.Tasks;

namespace AVS.CoreLib.Caching
{
    /// <summary>
    /// Represents a manager for caching between HTTP requests (long term caching)
    /// </summary>
    public interface IXCacheManager : IDisposable
    {
        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then <see cref="acquire"/> and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>The cached value associated with the specified key</returns>
        CachedObject<T> Get<T>(CacheKey key, Func<T> acquire);

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>The cached value associated with the specified key</returns>
        Task<CachedObject<T>> GetAsync<T>(CacheKey key, Func<Task<T>> acquire);

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it for a short term, if cache key has no CacheTime
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>The cached value associated with the specified key</returns>
        CachedObject<T> GetShortTerm<T>(CacheKey key, Func<T> acquire);

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it for a short term if cache key has no CacheTime
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>The cached value associated with the specified key</returns>
        Task<CachedObject<T>> GetShortTermAsync<T>(CacheKey key, Func<Task<T>> acquire);

        /// <summary>
        /// Puts the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="value">Value for caching</param>
        /// <param name="shortTerm">when true caching options ShortTermCacheTime is used as a default caching time</param>
        void Set<T>(CacheKey key, T value, bool shortTerm = false);

        /// <summary>
        /// If value exists in cache update it, otherwise do nothing
        /// </summary>
        void Refresh<T>(CacheKey key, T value, bool shortTerm = false);

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>True if item already is in cache; otherwise false</returns>
        bool IsSet(string key);

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        void Remove(string key);

        /// <summary>
        /// Removes items by key prefix
        /// </summary>
        /// <param name="prefix">String key prefix</param>
        void RemoveByPrefix(string prefix);

        /// <summary>
        /// Clear all cache data
        /// </summary>
        void Clear();
    }
}