using System.Collections.Generic;

namespace AVS.CoreLib.Caching
{
    /// <summary>
    /// Cache keys bookkeeper(ledger) is desgined for advanced caching scenarios when you need to track list of keys present in cache    /// 
    /// </summary>
    public interface ICacheKeysBookkeeper
    {
        void AddKey(string key);
        void Remove(string key);
        IEnumerable<string> GetKeys();
    }
}