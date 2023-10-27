using System;
using AVS.CoreLib.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace AVS.CoreLib.Caching
{
    /// <summary>
    /// cache entry (<see cref="T"/> data) wrapper
    /// Cache manager puts/gets data into/from cache wrapped into cached object
    /// this helps to determine how old the cached data, whether it was just acquired or taken from the cache
    /// It implicitly unwraps to <see cref="T"/> data 
    /// </summary>
    /// <remarks>actually this is a wrapper over <see cref="ICacheEntry"/> wrapper used by <see cref="IMemoryCache"/> to put data into cache</remarks>
    public class CachedObject<T>
    {
        public T Data { get; set; }
        /// <summary>
        /// timestamp when cached object created
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// indicates whether <see cref="Data"/> is taken from cache or been just acquired
        /// </summary>
        public bool FromCache => (DateTimeProvider.GetTime() - Timestamp).TotalMilliseconds > 500;
        public bool IsNullOrEmpty => Data == null || Data.Equals(default);
        public CachedObject(T data)
        {
            Data = data;
            Timestamp = DateTimeProvider.GetTime();
        }

        public static implicit operator T(CachedObject<T> result)
        {
            return result.Data;
        }

        public static explicit operator CachedObject<T>(T data)
        {
            return new CachedObject<T>(data);
        }        
    }
}