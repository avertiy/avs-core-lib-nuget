using System.Collections;
using System.Collections.Generic;

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

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Data.Add(item);
        }

        public void Clear()
        {
            Data.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Data.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
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

        public bool ContainsKey(TKey key)
        {
            return Data.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return Data.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Data.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => Data[key];
            set => Data[key] = value;
        }

        public ICollection<TKey> Keys => Data.Keys;

        protected bool ShouldSerializeKeys() => false;

        public ICollection<TValue> Values => Data.Values;

        protected bool ShouldSerializeValues() => false;
    }
}
