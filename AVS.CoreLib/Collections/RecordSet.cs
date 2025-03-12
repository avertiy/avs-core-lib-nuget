using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Abstractions.Collections;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.Extensions.Enums;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Collections
{
    /// <summary>
    /// Represents a wrapper over <see cref="List{T}"/> collection, helps to merge items, preserving their uniqueness and ascending/descending sort order
    /// </summary>
    public class RecordSet<T> : IRecordSet<T>
    {
        /// <summary>
        /// Comparer compares items by Timestamp, in most of the cases when timestamp is equal it means a duplicate record
        /// </summary>
        private readonly IComparer<T> _comparer;

        public Sort Sort { get; set; } = Sort.Desc;

        public List<T> Records { get; }

        public int Count => Records.Count;
        public bool IsReadOnly => false;

        public Func<T, T, int>? OnDuplicate = null;

        public RecordSet(int capacity, IComparer<T> comparer)
        {
            _comparer = comparer;
            Records = new List<T>(capacity);
        }

        public T this[int index]
        {
            get => Records[index];
            set => Records[index] = value;
        }

        public void Add(T item)
        {
            if (Records.Count == 0)
            {
                Records.Add(item);
                return;
            }

            var compare = Compare(item, Records[^1]);

            if (compare == 0)
                return;

            if (compare < 0)
            {
                Records.Add(item);
                return;
            }

            // search where to insert
            for (var i = 0; i < Records.Count - 1; i++)
            {
                compare = Compare(item, Records[i]);

                if (compare == 0)
                    break;

                if (compare > 0)
                {
                    Records.Insert(i, item);
                    break;
                }
            }
        }

        public int AddRange(IList<T> items)
        {
            if (items.Count == 0)
                return 0;

            Guard.Array.MustBeSorted(items, Sort, _comparer, items.Count < 10 ? items.Count : 10);

            Records.EnsureCapacity(Records.Count + items.Count);

            if (Records.Count == 0)
            {
                Records.AddRange(items);
                return items.Count;
            }

            int compare = Compare(items[0], Records[^1]);

            if (compare < 0)
            {
                Records.AddRange(items);
                return items.Count;
            }

            compare = Compare(items[^1], Records[^1]);

            if (compare < 0)
            {
                var olderItems = items.Skip(1).Where(x => Compare(x, Records[^1]) < 0).ToArray();

                if (olderItems.Length > 0)
                    Records.AddRange(olderItems);

                return olderItems.Length;
            }

            compare = Compare(items[0], Records[0]);

            if (compare > 0)
            {
                var newestItems = items.Where(x => Compare(x, Records[0]) > 0).ToArray();
                Records.InsertRange(newestItems);
                return newestItems.Length;
            }

            return 0;
        }

        public int Compare(T x, T y)
        {
            var compare = _comparer.Compare(x, y);

            if (compare == 0)
                compare = OnDuplicate?.Invoke(x, y) ?? 0;

            if (compare == 0)
                return 0;

            return Sort == Sort.Desc ? compare : -compare;
        }

        public IEnumerator<T> GetEnumerator() => Records.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int IndexOf(T item) => Records.IndexOf(item);

        public void RemoveAt(int index)
        {
            Records.RemoveAt(index);
        }

        public bool Remove(T item) => Records.Remove(item);

        public void Clear()
        {
            Records.Clear();
        }

        public bool Contains(T item) => Records.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            Records.CopyTo(array, arrayIndex);
        }

        public T[] ToArray() => Records.ToArray();
    }
}
