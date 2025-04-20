using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Extensions.Collections
{
    public static class ListExtensions
    {
        #region AddRange extensions
        /// <summary>
        /// Adds the elements of the given collection one-by-one to the end of this list.
        /// </summary>        
        public static int AddRange<T>(this IList<T> list, params T[] items)
        {
            if (items.Length == 0)
                return 0;

            var counter = 0;//for debug purposes
            foreach (var item in items)
            {
                list.Add(item);
                counter++;
            }

            return counter;
        }

        /// <summary>
        /// Adds the elements of the given collection one-by-one to the end of this list.
        /// </summary>        
        public static int AddRange<T>(this IList<T> list, IEnumerable<T> collection, bool distinct = false)
        {
            var counter = 0;
            foreach (var item in collection)
            {
                list.Add(item);
                counter++;
            }

            return counter;
        }

        /// <summary>
        /// Adds distinct elements one-by-one to the end of this list.
        /// </summary>        
        public static int AddDistinct<T>(this IList<T> list, params T[] items)
        {
            if (items.Length == 0)
                return 0;

            var counter = 0;
            foreach (var item in items)
            {
                if (list.Contains(item))
                    continue;

                list.Add(item);
                counter++;
            }
            return counter;
        }

        /// <summary>
        /// Adds distinct elements one-by-one to the end of this list.
        /// </summary>        
        public static int AddDistinct<T>(this IList<T> list, IEnumerable<T>? collection)
        {
            if (collection == null)
                return 0;

            var counter = 0;
            foreach (var item in collection)
            {
                if (list.Contains(item))
                    continue;

                list.Add(item);
                counter++;
            }
            return counter;
        }

        /// <summary>
        /// Adds unique by key items of the given collection to the end of the source list.        
        /// </summary>
        public static int AddRange<T, TKey>(this IList<T> source, IEnumerable<T> items, Func<T, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>(source.Select(keySelector));
            var counter = 0;
            foreach (var item in items)
            {
                var itemKey = keySelector(item);
                if (knownKeys.Add(itemKey))
                {
                    source.Add(item);
                    counter++;
                }
            }
            return counter;
        }

        /// <summary>
        /// Adds unique by key items of the given collection to the end of the source list.
        /// if they are satisfying a predicate condition
        /// </summary>
        public static int AddRange<T, TKey>(this IList<T> target, IEnumerable<T> second, Func<T, TKey> keySelector, Func<T, bool> predicate)
        {
            var knownKeys = new HashSet<TKey>(target.Select(keySelector));
            var counter = 0;
            foreach (var item in second)
            {
                if (predicate(item) && knownKeys.Add(keySelector(item)))
                {
                    target.Add(item);
                    counter++;
                }
            }
            return counter;
        } 
        #endregion

        #region Remove extensions
        /// <summary>
        /// Removes elements from the list. 
        /// </summary>
        public static int RemoveMany<T>(this IList<T> source, params T[] elements)
        {
            var counter = 0;
            foreach (var element in elements)
            {
                if (source.Remove(element))
                    counter++;
            }
            return counter;
        }

        public static T[] RemoveMany<T>(this IList<T> source, int count, int fromIndex = 0)
        {
            if (fromIndex + count > source.Count)
                throw new ArgumentOutOfRangeException();

            if (fromIndex == 0 && count == source.Count)
            {
                var removedItems = source.ToArray();
                source.Clear();
                return removedItems;
            }
            else
            {
                var removedItems = new T[count];

                for (var i = count - 1; i >= 0; i--)
                {
                    var ind = fromIndex + i;
                    removedItems[i] = source[ind];
                    source.RemoveAt(ind);
                }

                return removedItems;
            }
        }

        public static void RemoveByIndexes<T>(this IList<T> source, int[] indexes)
        {
            foreach (var ind in indexes.OrderByDescending(x => x))
            {
                source.RemoveAt(ind);
            }
        }
        #endregion

        #region FindIndex

        //public static int FindIndex<T>(this IList<T> source, Predicate<T> match)
        //{
        //    if (match == null)
        //        throw new ArgumentNullException(nameof(match));

        //    for (int i = 0; i < this.Count; i++)
        //    {
        //        if (match(this[i]))
        //            return i;
        //    }

        //    return -1; // Return -1 if no match is found
        //}

        public static int FindIndex<T>(this IList<T> source, int startIndex, Predicate<T> match)
        {
            return FindIndex<T>(source, startIndex, source.Count - startIndex, match);
        }

        /// <summary>
        /// Same implementation as List{T}.FindIndex
        /// </summary>
        //[Obsolete("Use Array.FindIndex(arr, startIndex, count, predicate)")]
        public static int FindIndex<T>(this IList<T> source, int startIndex, int count, Predicate<T> match)
        {
            if ((uint)startIndex > (uint)source.Count)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} must be less than {source.Count}");

            if (count < 0 || startIndex > source.Count - count)
                throw new ArgumentOutOfRangeException(nameof(count));

            var endIndex = startIndex + count;

            for (var i = startIndex; i < endIndex; i++)
            {
                if (match(source[i]))
                    return i;
            }

            return -1;
        }

        public static int[] FindIndexes1(this IList<decimal> source, decimal qty, int startIndex = 0, int limitRecursion = 0)
        {
            for (var i = startIndex; i < source.Count; i++)
            {
                if (source[i] > qty)
                    continue;

                if (source[i] == qty)
                    return new[] { i };

                if (limitRecursion > 0 && limitRecursion <= 1)
                    continue;

                var rest = qty - source[i];

                var arr = FindIndexes1(source, rest, i + 1, limitRecursion - 1);

                if (arr.Any())
                    return arr.Insert(i);
            }

            return Array.Empty<int>();
        }

        public static int FindIndex2<T>(this IList<T> source, int startIndex, int count, Predicate<(int index, T item)> match)
        {
            Guard.MustBe.GreaterThanOrEqual(startIndex, 0);
            Guard.MustBe.LessThan(startIndex, source.Count, $"{nameof(startIndex)} exceeds number of elements");

            var endIndex = startIndex + count;
            endIndex = source.Count < endIndex ? source.Count : endIndex;
            for (var i = startIndex; i < endIndex; i++)
            {
                if (match((i, source[i])))
                    return i;
            }
            return -1;
        }
        #endregion

        public static bool ContainsAll<T>(this IList<T> source, params T[] items)
        {
            if (source.Count < items.Length)
                return false;

            var result = true;

            foreach (var item in items)
            {
                if (!source.Contains(item))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
        
        public static IList<T> Shrink<T>(this IList<T> items, Func<T, double> selector, double threshold = 0.0)
        {
            var avg = items.Average(selector);
            if (threshold <= 0)
                threshold = avg;
            return items.Where(i => selector(i) >= threshold).ToList();
        }

        /// <summary>
        /// Select element(s) by their respective indices
        /// </summary>
        public static T[] ElementsAt<T>(this IList<T> source, params int[] indices)
        {
            return indices.Select(i => source[i]).ToArray();
        }

        /// <summary>
        /// Inserts elements of the other list at the beginning of the source list
        /// It`s an optimized equivalent of <see cref="List{T}.InsertRange"/>
        /// </summary>
        public static void InsertRange<T>(this IList<T> source, IList<T> other)
        {
            if (other.Count == 0)
                return;

            if (other.Count == 1)
            {
                source.Insert(0, other[0]);
                return;
            }

            var size = source.Count + other.Count;
            var arr = new T[size];

            // copy elements of the other list to a new array
            other.CopyTo(arr, 0);

            // Copy the original list's elements into the new array after the inserted items
            source.CopyTo(arr, other.Count);
            source.Clear();
            source.AddRange(arr);
        }

        /// <summary>
        /// Merge source with another collection into a new list of items, dropping duplicates
        /// </summary>
        public static List<T> Merge<T, TKey>(this IEnumerable<T> source, IEnumerable<T> other, Func<T, TKey> key, Func<(T source, T other), T>? resolve = null) where TKey : notnull
        {
            var dict = new Dictionary<TKey, T>();
            foreach (var item in source)
            {
                var itemKey = key(item);
                if (dict.ContainsKey(itemKey))
                {
                    dict[itemKey] = resolve != null ? resolve.Invoke((dict[itemKey], item)) : dict[itemKey];
                }
                else
                {
                    dict[itemKey] = item;
                }
            }

            foreach (var item in other)
            {
                var itemKey = key(item);
                if (dict.ContainsKey(itemKey))
                {
                    dict[itemKey] = resolve != null ? resolve.Invoke((dict[itemKey], item)) : dict[itemKey];
                }
                else
                {
                    dict[itemKey] = item;
                }
            }

            return dict.Values.ToList();
        }
    }
}