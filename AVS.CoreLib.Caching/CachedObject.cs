using System;

namespace AVS.CoreLib.Caching
{
    public struct CachedObject<T>
    {
        public T Data;
        public DateTime Timestamp;
        public bool FromCache => (DateTime.Now - Timestamp).TotalMilliseconds > 500;
        public bool IsNullOrEmpty => Data == null || Data.Equals(default);
        public CachedObject(T data)
        {
            Data = data;
            Timestamp = DateTime.Now;
        }

        public static implicit operator T(CachedObject<T> result)
        {
            return result.Data;
        }
    }
}