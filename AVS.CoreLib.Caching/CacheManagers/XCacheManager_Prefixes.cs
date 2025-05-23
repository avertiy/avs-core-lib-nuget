﻿using System.Collections.Concurrent;
using System.Threading;

namespace AVS.CoreLib.Caching
{
    public partial class XCacheManager : CacheManagerBase, IXCacheManager
    {

        private readonly ConcurrentDictionary<string, CancellationTokenSource> _prefixes = new();

        /// <summary>
        /// Removes items by key prefix
        /// </summary>
        /// <param name="prefix">String key prefix</param>
        public void RemoveByPrefix(string prefix)
        {
            _prefixes.TryRemove(prefix, out var tokenSource);
            tokenSource?.Cancel();
            tokenSource?.Dispose();
        }
    }
}