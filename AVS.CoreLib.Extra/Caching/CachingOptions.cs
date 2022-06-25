namespace AVS.CoreLib.Caching
{
    public class CachingOptions
    {
        public bool CachingEnabled { get; set; } = true;
        /// <summary>
        /// Gets or sets the default cache time in minutes
        /// </summary>
        public int DefaultCacheTime { get; set; }

        /// <summary>
        /// Gets or sets the short term cache time in minutes
        /// </summary>
        public int ShortTermCacheTime { get; set; }
    }
}