namespace AVS.CoreLib.Caching
{
    public interface ICacheable
    {
        /// <summary>
        /// when less or equal to 0 the cache is not used
        /// if null the default cache time or short term cache time is applied 
        /// </summary>
        int? CacheTime { get; }
    }
}