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
        protected Dictionary<string, object> Bag { get; }
        protected Dictionary<string, T> TypedBag { get; }

        public Box()
        {
            Bag = new();
            TypedBag = new();
        }

        public Box(int capacity, int typedCapacity)
        {
            Bag = new(capacity);
            TypedBag = new(typedCapacity);
        }

        public void InitFrom(Box<T> box)
        {
            foreach (var kp in box.TypedBag)
            {
                TypedBag[kp.Key] = kp.Value;
            }

            foreach (var kp in box.Bag)
            {
                Bag[kp.Key] = kp.Value;
            }
        }

        public void EnsureCapacity(int capacity)
        {
            Bag.EnsureCapacity(capacity);
        }

        public void EnsureTypedCapacity(int capacity)
        {
            Bag.EnsureCapacity(capacity);
        }

        
        public bool ContainsKey(string key)
        {
            return Bag.ContainsKey(key);
        }

        public bool ContainsTypedKey(string key)
        {
            return TypedBag.ContainsKey(key);
        }

        public T Get(string key)
        {
            return TypedBag[key];
        }

        public T Get(string key, Func<T> acquire)
        {
            if (TypedBag.TryGetValue(key, out var val))
                return val;

            return TypedBag[key] = acquire();
        }

        public bool TryGetValue(string key, out T value)
        {
            if (TypedBag.TryGetValue(key, out value))
            {
                return true;
            }

            value = default;
            return false;
        }

        public TValue Get<TValue>(string key)
        {
            return (TValue)Bag[key];
        }

        public TValue? GetOrDefault<TValue>(string key)
        {
            return Bag.ContainsKey(key) ? (TValue)Bag[key] : default;
        }

        public bool TryGetValue<TValue>(string key, out TValue? value)
        {
            if (Bag.TryGetValue(key, out var obj))
            {
                value = (TValue?)obj;
                return true;
            }

            value = default;
            return false;
        }

        public void Set(string key, T value)
        {
            TypedBag[key] = value;
        }

        public void Set(string key, object value)
        {
            Bag[key] = value;
        }

        public override string ToString()
        {
            var keys = string.Join(',', Bag.Keys.Take(10)).Truncate(30, TruncateOptions.CutOffTheMiddle);
            var typedKeys = string.Join(',', TypedBag.Keys.Take(10)).Truncate(30, TruncateOptions.CutOffTheMiddle);
            return $"Box ({TypedBag.Count}/{Bag.Count}) [{typedKeys};{keys}]";
        }
    }
}