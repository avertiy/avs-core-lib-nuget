namespace AVS.CoreLib.Caching
{
    /// <summary>
    /// represent a cache key with some extra parameters 
    /// </summary>
    public readonly struct CacheKey
    {
        /// <summary>
        /// Cache key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// To remove by prefix functionality
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// Cache time in minutes
        /// when 0 - force to acquire the value regardless the cache entry exists or not
        /// when null - default cache time is used
        /// <seealso cref="CachingOptions.DefaultCacheTime"/> and <seealso cref="CachingOptions.ShortTermCacheTime"/>
        /// </summary>
        public int? CacheTime { get; }

        /// <summary>
        /// force to acquire the value regardless the cache entry exists or not
        /// </summary>
        public bool ForceAcquire => CacheTime <= 0;

        public CacheKey(string cacheKey, string prefix = null, int? cacheTime = null)
        {
            CacheTime = cacheTime;
            Key = cacheKey;
            Prefix = prefix;
        }

        //public static implicit operator CacheKey(string cacheKey)
        //{
        //    return new CacheKey(cacheKey);
        //}
    }
}
