using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Caching
{
    /// <summary>
    /// cache keys bookkeeper (ledger) helps to keep records about keys, stored in cache by <see cref="CacheManager"/>
    /// </summary>
    public interface ICacheKeysBookkeeper
    {
        void Add(string key);
        void Remove(string key);
        IEnumerable<string> GetKeys();
    }

    public interface IKeysBookkeeper
    {
        ICacheKeysBookkeeper KeysBookkeeper { get; set; }
    }
}