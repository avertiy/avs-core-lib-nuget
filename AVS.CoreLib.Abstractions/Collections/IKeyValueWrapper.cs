using System.Collections.Generic;

namespace AVS.CoreLib.Abstractions.Collections
{
    /// <summary>
    /// <see cref="IDictionary{TKey,TValue}"/> interface is rarely implemented
    /// in most cases wrapper classes implement Add(TKey key, TValue value) method
    /// </summary>
    public interface IKeyValueWrapper<in TKey, in TValue>
    {
        void Add(TKey key, TValue value);
    }
}