using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Collections
{
    public abstract class BaseDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        protected IDictionary<TKey, TValue> Data;
        protected bool ShouldSerializeData() => false;

        protected BaseDictionary()
        {
            Data = new Dictionary<TKey, TValue>();
        }

        protected BaseDictionary(int capacity)
        {
            Data = new Dictionary<TKey, TValue>(capacity);
        }

        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            Data.Add(item);
        }

        public virtual void Clear()
        {
            Data.Clear();
        }

        public virtual bool Contains(TKey key, TValue value)
        {
            return Data.Contains(new KeyValuePair<TKey, TValue>(key, value));
        }

        public virtual bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Data.Contains(item);
        }

        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Data.Remove(item);
        }

        public int Count => Data.Count;

        public virtual bool ShouldSerializeCount()
        {
            return Count > 1;
        }

        public bool IsReadOnly => Data.IsReadOnly;

        public virtual bool ShouldSerializeIsReadOnly() => false;

        public virtual void Add(TKey key, TValue value)
        {
            Data.Add(key, value);
        }

        public virtual bool ContainsKey(TKey key)
        {
            return Data.ContainsKey(key);
        }

        public virtual bool Remove(TKey key)
        {
            return Data.Remove(key);
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            return Data.TryGetValue(key, out value);
        }

        public virtual TValue this[TKey key]
        {
            get => Data[key];
            set => Data[key] = value;
        }

        public virtual ICollection<TKey> Keys => Data.Keys;

        protected virtual bool ShouldSerializeKeys() => false;

        public virtual ICollection<TValue> Values => Data.Values;

        protected virtual bool ShouldSerializeValues() => false;

        public override string ToString()
        {
            return Data.Stringify();
        }
    }
}
