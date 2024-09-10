using System;
using System.Collections.Generic;
using AVS.CoreLib.Abstractions;

namespace AVS.CoreLib.Caching
{
    /// <summary>
    /// Keys Bookkeeper(ledger) is designed for advanced caching scenarios 
    /// when you need to track list of keys present in cache
    /// </summary>
    public class KeysBookkeeper : ICacheKeysBookkeeper
    {
        private readonly List<string> _keys = new List<string>();
        private readonly ICacheManager _cacheManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private DateTime _updated;

        public KeysBookkeeper(ICacheManager cacheManager, IDateTimeProvider dateTimeProvider = null)
        {
            if (cacheManager is CacheManagerBase mgr)
            {
                mgr.ItemRemoved += OnItemRemoved;
                mgr.ItemAdded += OnItemAdded;
            }

            _dateTimeProvider = dateTimeProvider ?? DateTimeProvider.Instance;
            _updated = _dateTimeProvider.GetSystemTime();
            _cacheManager = cacheManager;
        }

        private void OnItemAdded(string key, object item)
        {
            this.AddKey(key);
        }

        private void OnItemRemoved(string key)
        {
            this.Remove(key);
        }

        private TimeSpan TimeSinceLastUpdate => _dateTimeProvider.GetSystemTime() - _updated;

        /// <summary>
        /// in minutes
        /// </summary>
        public int UpdateInterval { get; set; } = 15;

        public void AddKey(string key)
        {
            if (!_keys.Contains(key))
                _keys.Add(key);

            CleanUp();
        }

        public void Remove(string key)
        {
            _keys.Remove(key);
        }

        private void CleanUp()
        {
            if (_keys.Count < 100 || TimeSinceLastUpdate.TotalMinutes < UpdateInterval)
                return;

            foreach (var key in _keys.ToArray())
            {
                if (!_cacheManager.IsSet(key))
                    _keys.Remove(key);
            }

            _updated = _dateTimeProvider.GetSystemTime();
        }

        public IEnumerable<string> GetKeys()
        {
            foreach (var key in _keys.ToArray())
            {
                if (!_cacheManager.IsSet(key))
                {
                    _keys.Remove(key);
                    continue;
                }

                yield return key;
            }

            _updated = _dateTimeProvider.GetSystemTime();
        }
    }
}