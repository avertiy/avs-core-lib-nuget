namespace AVS.CoreLib.Caching
{
    /// <summary>
    /// wrapper helps to sort it out whether the data was taken from cache or acquired/fetched
    /// </summary>
    public struct CacheResult<T>
    {
        public T Data;
        public bool FromCache;

        public CacheResult(T data, bool fromCache = false)
        {
            Data = data;
            FromCache = fromCache;
        }

        public static implicit operator T(CacheResult<T> result)
        {
            return result.Data;
        }

        public static implicit operator CacheResult<T>(T result)
        {
            return new CacheResult<T>(result);
        }

        public static implicit operator bool(CacheResult<T> result)
        {
            return result.FromCache;
        }

        public override string ToString()
        {
            var str = Data?.ToString() ?? "";
            return FromCache ? $"{str} (FROM CACHE)" : str;
        }
    }
}