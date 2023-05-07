using System;
using System.Collections.Generic;
using AVS.CoreLib.Abstractions;

namespace AVS.CoreLib.Caching
{
    /// <summary>
    /// cache keys ledger or bookkeeper helps to keep records about keys stored by cache manager <see cref="_cacheManager"/>
    /// note the <see cref="ICacheManager"/> must implement <see cref="IKeysBookkeeper"/> interface
    /// </summary>
    public class CacheKeysBookkeeper : ICacheKeysBookkeeper
    {
        private readonly List<string> _keys = new List<string>();
        private readonly ICacheManager _cacheManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private DateTime _updated;

        public CacheKeysBookkeeper(ICacheManager cacheManager, IDateTimeProvider dateTimeProvider = null)
        {
            if (cacheManager is IKeysBookkeeper kb)
            {
                kb.KeysBookkeeper = this;
            }

            _dateTimeProvider = dateTimeProvider ?? DateTimeProvider.Instance;
            _updated = _dateTimeProvider.GetSystemTime();
            _cacheManager = cacheManager;
        }

        private TimeSpan TimeSinceLastUpdate => _dateTimeProvider.GetSystemTime() - _updated;

        /// <summary>
        /// in minutes
        /// </summary>
        public int UpdateInterval { get; set; } = 15;
        
        public void Add(string key)
        {
            if (!_keys.Contains(key))
                _keys.Add(key);

            UpdateKeyRecords();
        }

        public void Remove(string key)
        {
            _keys.Remove(key);
        }

        private void UpdateKeyRecords()
        {
            if(_keys.Count < 100 || TimeSinceLastUpdate.TotalMinutes < UpdateInterval) 
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