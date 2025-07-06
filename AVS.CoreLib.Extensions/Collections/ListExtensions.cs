using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions.Enums;
using AVS.CoreLib.Extensions.Linq;
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

        #region AddDistinct

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

        #endregion

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

        /// <summary>
        /// Helps to find index
        /// (same implementation as in List{T}.FindIndex)
        /// </summary>
        public static int FindIndex<T>(this IList<T> source, Predicate<T> match, int startIndex = 0, int count = 0)
        {
            Guard.MustBe.WithinRange(startIndex, 0, source.Count, nameof(startIndex));
            Guard.MustBe.WithinRange(count, 0, source.Count-startIndex, nameof(count));
            
            var endIndex = count > 0 ? startIndex + count : source.Count;

            for (var i = startIndex; i < endIndex; i++)
            {
                if (match(source[i]))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Helps to find index 
        /// </summary>
        /// <remarks>FindIndex2 helps to resolve collisions with .net FindIndex</remarks>
        public static int FindIndex2<T>(this IList<T> source, Predicate<(int index, T item)> match, int startIndex, int count)
        {
            Guard.MustBe.WithinRange(startIndex, 0, source.Count, nameof(startIndex));
            Guard.MustBe.WithinRange(count, 0, source.Count - startIndex, nameof(count));

            var endIndex = count > 0 ? startIndex + count : source.Count;

            for (var i = startIndex; i < endIndex; i++)
            {
                if (match((i, source[i])))
                    return i;
            }

            return -1;
        }

        //public static int FindIndex2<T>(this IList<T> source, Predicate<(int index, T item)> match, int startIndex, int count)

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
        
        public static bool Any<T>(this IEnumerable<T> source, Func<T, bool> predicate, Direction direction = Direction.Forward)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(predicate, nameof(predicate));

            if (direction == Direction.Reverse)
            {
                var list = source as IList<T> ?? new List<T>(source);
                for (var i = list.Count - 1; i >= 0; i--)
                {
                    if (predicate(list[i]))
                        return true;
                }
                return false;
            }

            // Forward (standard) case
            foreach (var item in source)
            {
                if (predicate(item))
                    return true;
            }

            return false;
        }
    }
}