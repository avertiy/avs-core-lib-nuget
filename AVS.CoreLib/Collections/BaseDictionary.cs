using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Stringify;

namespace AVS.CoreLib.Collections
{
    [DebuggerDisplay("{ToString()}")]
    public abstract class BaseDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
    {
        private Dictionary<TKey, TValue> _data;
        protected IDictionary<TKey, TValue> Data => _data;
        protected virtual bool ShouldSerialize_Data() => false;
        public int Count => _data.Count;
        public bool IsReadOnly => Data.IsReadOnly;
        public virtual int Capacity => _data.Capacity;

        protected BaseDictionary()
        {
            _data = new Dictionary<TKey, TValue>();
        }

        protected BaseDictionary(int capacity)
        {
            _data = new Dictionary<TKey, TValue>(capacity);
        }

        protected BaseDictionary(SerializationInfo info, StreamingContext context)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Ensures that the dictionary can hold up to 'capacity' entries without any further expansion of its backing storage
        /// </summary>
        public virtual int EnsureCapacity(int capacity)
        {
            return _data.EnsureCapacity(capacity);
        }

        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            Data.Add(item);
        }

        public virtual void Clear()
        {
            _data.Clear();
        }

        public virtual bool Contains(TKey key, TValue value)
        {
            return Data.Contains(new KeyValuePair<TKey, TValue>(key, value));
        }

        public virtual bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _data.Contains(item);
        }

        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _data.Remove(item.Key);
        }

        

        public virtual bool ShouldSerializeCount()
        {
            return Count > 1;
        }

        

        public virtual bool ShouldSerializeIsReadOnly() => false;

        public virtual void Add(TKey key, TValue value)
        {
            _data.Add(key, value);
        }

        public virtual bool ContainsKey(TKey key)
        {
            return _data.ContainsKey(key);
        }

        public virtual bool Remove(TKey key)
        {
            return _data.Remove(key);
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            return _data.TryGetValue(key, out value);
        }

        public virtual TValue this[TKey key]
        {
            get => _data[key];
            set => _data[key] = value;
        }

        public virtual ICollection<TKey> Keys => _data.Keys;

        protected virtual bool ShouldSerializeKeys()
        {
            return false;
        }

        public virtual ICollection<TValue> Values => _data.Values;

        protected virtual bool ShouldSerializeValues()
        {
            return false;
        }


        public override string ToString()
        {
            var str = AsString().Truncate();
            return $"Dicitonary<{typeof(TKey).Name},{typeof(TValue).Name} > #{Count} of {Capacity} {str}";
        }

        public string AsString()
        {
            return _data.Stringify(options: default);
        }
    }
}
