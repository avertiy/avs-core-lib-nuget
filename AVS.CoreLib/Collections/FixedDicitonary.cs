using System;
using System.Collections.Generic;
using System.Diagnostics;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Collections
{
    /// <summary>
    /// Represents a fixed-size dictionary
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public class FixedDicitonary<TKey, TValue> : BaseDictionary<TKey, TValue> where TKey : notnull
    {
        private FixedList<TKey> _queue;

        public int MaxCapacity { get; }

        public FixedDicitonary(int capacity) : this (capacity, capacity) { }

        public FixedDicitonary(int capacity, int maxCapacity) : base(capacity)
        {
            Guard.MustBe.GreaterThanOrEqual(maxCapacity, capacity);
            _queue = new FixedList<TKey>(capacity);
            MaxCapacity = maxCapacity;
        }

        public override int EnsureCapacity(int capacity)
        {
            _queue.EnsureCapacity(capacity);
            return base.EnsureCapacity(capacity);
        }

        public override void Add(TKey key, TValue value)
        {
            if (Count >= MaxCapacity)
                throw new InvalidOperationException($"Dicitonary reached a max number of elements (capacity:{MaxCapacity})");

            if (Count == Capacity)
            {
                var newSize = Math.Min(Capacity * 2, MaxCapacity);
                EnsureCapacity(newSize);
            }
                

            base.Add(key, value);
            _queue.Enqueue(key);
        }

        public bool Upsert(TKey key, TValue value)
        {
            if (Data.ContainsKey(key))
            {
                Data[key] = value;
                _queue.Put(key);
                return false;
            }

            if (Count == Capacity)
            {
                var newSize = Math.Min(Capacity * 2, MaxCapacity);
                EnsureCapacity(newSize);
            }

            if (Count == MaxCapacity)
            {
                var evictedKey = _queue.Dequeue();
                Data.Remove(evictedKey);
            }

            base.Add(key, value);
            _queue.Enqueue(key);
            return true;
        }

        public override void Clear()
        {
            _queue.Clear();
            base.Clear();
        }

        public override bool Remove(TKey key)
        {
            _queue.Remove(key);
            return base.Remove(key);
        }

        public override bool Remove(KeyValuePair<TKey, TValue> item)
        {
            _queue.Remove(item.Key);
            return base.Remove(item);
        }

        public override ICollection<TKey> Keys => _queue;


        public override string ToString()
        {
            var str = AsString().Truncate();
            return $"FixedDicitonary<{typeof(TKey).Name},{typeof(TValue).Name} > #{Count} of {Capacity} {str}";
        }
    }
}
