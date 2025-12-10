using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Types
{
    /// <summary>
    /// Represents a box of values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record Box<T> where T : struct
    {
        private readonly Dictionary<string, object> _bag;
        private readonly Dictionary<string, T> _tBag;

        public Box()
        {
            _bag = new();
            _tBag = new();
        }

        public Box(int capacity, int typedCapacity)
        {
            _bag = new(capacity);
            _tBag = new(typedCapacity);
        }

        protected IDictionary<string, object> GetBag()
        {
            return _bag;
        }

        protected IDictionary<string, T> GetTypedBag()
        {
            return _tBag;
        }

        public void InitFrom(Box<T> box)
        {
            foreach (var kp in box._tBag)
            {
                _tBag[kp.Key] = kp.Value;
            }

            foreach (var kp in box._bag)
            {
                _bag[kp.Key] = kp.Value;
            }
        }

        public void EnsureCapacity(int capacity)
        {
            _bag.EnsureCapacity(capacity);
        }

        public void EnsureTypedCapacity(int capacity)
        {
            _tBag.EnsureCapacity(capacity);
        }
        
        public bool ContainsKey(string key)
        {
            return _bag.ContainsKey(key);
        }

        public bool ContainsTypedKey(string key)
        {
            return _tBag.ContainsKey(key);
        }

        public T Get(string key)
        {
            return _tBag[key];
        }

        public T Get(string key, Func<T> acquire)
        {
            if (_tBag.TryGetValue(key, out var val))
                return val;

            return _tBag[key] = acquire();
        }

        public bool TryGetValue(string key, out T value)
        {
            if (_tBag.TryGetValue(key, out value))
            {
                return true;
            }

            value = default;
            return false;
        }

        public TValue Get<TValue>(string key)
        {
            return (TValue)_bag[key];
        }

        public TValue? GetOrDefault<TValue>(string key)
        {
            return _bag.ContainsKey(key) ? (TValue)_bag[key] : default;
        }

        public bool TryGetValue<TValue>(string key, out TValue? value)
        {
            if (_bag.TryGetValue(key, out var obj))
            {
                value = (TValue?)obj;
                return true;
            }

            value = default;
            return false;
        }

        public void Set(string key, T value)
        {
            _tBag[key] = value;
        }

        public void Set(string key, object value)
        {
            _bag[key] = value;
        }

        public override string ToString()
        {
            var keys = string.Join(',', _bag.Keys.Take(10)).Truncate(30, TruncateOptions.CutOffTheMiddle);
            var typedKeys = string.Join(',', _tBag.Keys.Take(10)).Truncate(30, TruncateOptions.CutOffTheMiddle);
            return $"Box ({_tBag.Count}/{_bag.Count}) [{typedKeys};{keys}]";
        }
    }
}