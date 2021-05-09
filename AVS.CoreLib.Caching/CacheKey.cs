namespace AVS.CoreLib.Caching
{
    public readonly struct CacheKey
    {
        /// <summary>
        /// Cache time in minutes
        /// when null <see cref="CachingOptions.DefaultCacheTime"/> or <seealso cref="CachingOptions.ShortTermCacheTime"/>
        /// </summary>
        public int? CacheTime { get; }

        /// <summary>
        /// Cache key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// To remove by prefix functionality
        /// </summary>
        public string Prefix { get;  }

        public CacheKey(string cacheKey, string prefix = null, int? cacheTime =null)
        {
            CacheTime = cacheTime;
            Key = cacheKey;
            Prefix = prefix;
        }

        public static implicit operator CacheKey(string cacheKey)
        {
            return new CacheKey(cacheKey);
        }
    }
}
