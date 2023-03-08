using System;
using System.Threading.Tasks;

namespace AVS.CoreLib.Caching
{
    public interface ICacheManager : IDisposable
    {
        /// <summary>
        /// default cache duration (in minutes)
        /// </summary>
        int DefaultCacheDuration { get; set; }
        bool CachingEnabled { get; set; }

        /// <summary>
        /// Get <see cref="T"/> value from cache by the given key. If value is not present in cache acquire it with <see cref="acquire"/> callback.
        /// If <see cref="cacheDuration"/>(in minutes) is positive puts the value into cache 
        /// </summary>
        Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheDuration = null);

        bool TryGetValue<T>(string key, out T value);

        void Set<T>(string key, T value, int? cacheDuration = null);

        bool IsSet(string key);

        /// <summary>
        /// If value exists in cache refresh(update) it, otherwise skip
        /// </summary>
        void Refresh<T>(string key, T value, int? cacheDuration = null);

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        void Remove(string key);

        /// <summary>
        /// Clear all cache data
        /// </summary>
        void Clear();
    }
}