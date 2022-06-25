using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using AVS.CoreLib.Dates.Extensions;

namespace AVS.CoreLib.Collections
{
    public class TimeKeyedBag<T> : IEnumerable<KeyValuePair<DateTime, StrongBox<T>>>
        where T : class, new()
    {
        private readonly object _lock = new object();
        private readonly Dictionary<DateTime, StrongBox<T>> _items = new Dictionary<DateTime, StrongBox<T>>();

        public T this[DateTime time, int roundToSeconds = 60]
        {
            get
            {
                var key = time.Round(roundToSeconds);
                EnsureKeyCreated(key);
                return _items[key].Value;
            }
        }

        public T[] Values => _items.Values.Select(x => x.Value).ToArray();
        public Dictionary<DateTime, StrongBox<T>>.KeyCollection Keys => _items.Keys;

        public void AddOrUpdate(DateTime key, T value)
        {
            var added = false;
            if (!_items.ContainsKey(key))
                lock (_lock)
                {
                    var box = new StrongBox<T>(value);
                    if (!_items.ContainsKey(key))
                    {
                        _items.Add(key, box);
                        added = true;
                    }
                }

            if (!added)
            {
                SetValue(key, value);
            }
        }

        public bool ContainsKey(DateTime key)
        {
            return _items.ContainsKey(key);
        }

        public void SetValue(DateTime key, T value)
        {
            var box = _items[key];
            Interlocked.Exchange(ref box.Value, value);
        }

        public T GetValue(DateTime key)
        {
            return _items[key].Value;
        }

        private void EnsureKeyCreated(DateTime key)
        {
            if (!_items.ContainsKey(key))
                lock (_lock)
                {
                    var box = new StrongBox<T>(new T());
                    if (!_items.ContainsKey(key))
                        _items.Add(key, box);
                }
        }

        public IEnumerator<KeyValuePair<DateTime, StrongBox<T>>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}