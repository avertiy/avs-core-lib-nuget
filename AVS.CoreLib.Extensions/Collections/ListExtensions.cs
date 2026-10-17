using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions.Enums;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Extensions.Collections
{
    /// <summary>
    /// Provides various extension methods for IList{T} to enhance its functionality
    /// </summary>
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
        /// Adds elements of the given source one-by-one to the list.
        /// </summary>        
        public static void AddRange<T>(this IList<T> list, IList<T> source, int startIndex, int count)
        {
            for (var i = startIndex; i < startIndex+ count; i++)
                list.Add(source[i]);
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
        /// Removes the last element from the list.
        /// </summary>
        public static void RemoveLast<T>(this IList<T> source)
        {
            source.RemoveAt(source.Count - 1);
        }

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

        /// <summary>
        /// Removes elements from the list. 
        /// </summary>
        public static int RemoveMany<T>(this IList<T> source, IList<T> elements)
        {
            var counter = 0;
            foreach (var element in elements)
            {
                if (source.Remove(element))
                    counter++;
            }
            return counter;
        }

        /// <summary>
        /// Removes multiple elements from the list. 
        /// </summary>
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

        /// <summary>
        /// Removes elements at respective indexes from the source list
        /// </summary>
        public static void RemoveByIndexes<T>(this IList<T> source, int[] indexes)
        {
            foreach (var ind in indexes.OrderByDescending(x => x))
            {
                source.RemoveAt(ind);
            }
        }

        /// <summary>
        /// Removes 2 elements at respective indexes  from the source list
        /// </summary>
        public static void RemoveByIndexes<T>(this IList<T> source, int index1, int index2)
        {
            if (index2 > index1)
            {
                source.RemoveAt(index2);
                source.RemoveAt(index1);
            }
            else
            {
                source.RemoveAt(index1);
                source.RemoveAt(index2);
            }   
        }
        #endregion

        #region FindIndex

        /// <summary>
        /// Helps to find index
        /// (same implementation as in List{T}.FindIndex)
        /// </summary>
        /// <summary>
        /// Finds the index of the first element that matches the specified predicate.
        /// </summary>
        /// <remarks>
        /// Same implementation as in List{T}.FindIndex.
        /// Differences from standard FindIndex extension: 
        ///     - predicate contains an index
        ///     - returns -1 if no matching element is found.
        /// </remarks>
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
        /// Finds an element matching the <paramref name="match"/> predicate,
        /// preferring an element that also matches the <paramref name="preferredMatch"/> predicate.
        /// If no matching element is found, returns -1.
        /// </summary>
        public static int FindIndex<T>(this IList<T> source, Predicate<T> match, Predicate<T> preferredMatch, int startIndex = 0, int count = 0)
        {
            Guard.MustBe.WithinRange(startIndex, 0, source.Count, nameof(startIndex));
            Guard.MustBe.WithinRange(count, 0, source.Count - startIndex, nameof(count));

            var endIndex = count > 0 ? startIndex + count : source.Count;

            var index = -1;

            for (var i = startIndex; i < endIndex; i++)
            {
                var item = source[i];

                if (!match(item))
                    continue;

                index = i;

                if (preferredMatch(item))
                    break;
            }

            return index;
        }

        /// <summary>
        /// Finds last element matching the <paramref name="match"/> predicate,
        /// preferring an element that also matches the <paramref name="preferredMatch"/> predicate.
        /// If no matching element is found, returns -1.
        /// </summary>
        public static int FindLastIndex<T>(this IList<T> source, Predicate<T> match, Predicate<T> preferredMatch, int offset = 0, int count = 0)
        {
            Guard.MustBe.WithinRange(offset, 0, source.Count, nameof(offset));
            Guard.MustBe.WithinRange(count, 0, source.Count - offset, nameof(count));

            var index = -1;
            var startIndex = source.Count - offset -1;
            var endIndex = startIndex - count;
            
            for (var i = startIndex; i > endIndex; i--)
            {
                var item = source[i];

                if (!match(item))
                    continue;

                index = i;

                if (preferredMatch(item))
                    break;
            }

            return index;
        }

        /// <summary>
        /// Finds last element matching the <paramref name="match"/> predicate,
        /// If no matching element is found, returns -1.
        /// </summary>
        public static int FindLastIndex<T>(this IList<T> source, Predicate<T> match, int offset = 0, int count = 0)
        {
            Guard.MustBe.WithinRange(offset, 0, source.Count, nameof(offset));
            Guard.MustBe.GreaterThanOrEqual(count, 0);

            var index = -1;
            var startIndex = source.Count - offset - 1;
            var endIndex = startIndex - count;

            if (endIndex < 0)
                endIndex = 0;

            for (var i = startIndex; i > endIndex; i--)
            {
                var item = source[i];

                if (!match(item))
                    continue;

                index = i;
                break;
            }

            return index;
        }


        /// <summary>
        /// Finds the index of the first element that matches the specified predicate.
        /// </summary>
        /// <remarks>
        /// FindIndex2 to resolve name collision with <see cref="FindIndex"/>
        /// Differences from standard FindIndex extension:
        ///     - predicate contains an index
        ///     - returns -1 if no matching element is found.
        /// </remarks>
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

        /// <summary>
        /// Finds an element matching the <paramref name="match"/> predicate,
        /// preferring an element that also matches the <paramref name="preferredMatch"/> predicate.
        /// If no matching element is found, returns -1.
        /// </summary>
        public static int FindIndex2<T>(this IList<T> source, Predicate<(int index, T item)> match, Predicate<(int index, T item)> preferredMatch, int startIndex = 0, int count = 0)
        {
            Guard.MustBe.WithinRange(startIndex, 0, source.Count, nameof(startIndex));
            Guard.MustBe.WithinRange(count, 0, source.Count - startIndex, nameof(count));

            var endIndex = count > 0 ? startIndex + count : source.Count;

            var index = -1;

            for (var i = startIndex; i < endIndex; i++)
            {
                var item = source[i];

                if (!match((i,item)))
                    continue;

                index = i;

                if (preferredMatch((i,item)))
                    break;
            }

            return index;
        }


        /// <summary>
        /// Finds few (2-4) elements mathing the <paramref name="match"/> predicate. 
        /// <code>
        /// usage:
        /// var arr = [1,2,3,4]
        /// var indexes = arr.FindIndexes(x => x.Sum() == 6);// => [1,3] i.e. 2+4
        /// </code>
        /// </summary>
        public static int[] FindIndexes<T>(this IList<T> source, Func<T[], bool> match, int startIndex = 0, int count = 0)
        {
            Guard.MustBe.WithinRange(startIndex, 0, source.Count, nameof(startIndex));
            Guard.MustBe.GreaterThanOrEqual(count, 0);

            var n = count > 0 && count < source.Count ? count : source.Count;
            var arr2 = new T[2];
            var arr3 = new T[3];
            var arr4 = new T[4];

            for (var i = startIndex; i < n - 1; i++)
            {
                arr2[0] = arr3[0] = arr4[0] = source[i];

                for (var j = i + 1; j < n; j++)
                {
                    arr2[1] = arr3[1] = arr4[1] = source[j];

                    if (match(arr2))
                        return [i,j];

                    for (var k = j + 1; k < n; k++)
                    {
                        arr3[2] = arr4[2] = source[k];

                        if (match(arr3))
                            return [i,j,k];

                        for (var l = k + 1; l < n; l++)
                        {
                            arr4[3] = source[l];
                            if (match(arr4))
                                return [i,j,k,l];
                        }
                    }

                }
            }

            return [];
        }

        /// <summary>
        /// Finds few (2-4) elements mathing the <paramref name="match"/> predicate. 
        /// <code>
        /// usage:
        /// var arr = [1,2,3,4]
        /// var indexes = arr.FindLastIndexes(x => x.Sum() == 5);// => [3,0]  i.e. 4+1
        /// </code>
        /// </summary>
        public static int[] FindLastIndexes<T>(this IList<T> source, Func<T[], bool> match, int offset = 0, int count = 0)
        {
            Guard.MustBe.WithinRange(offset, 0, source.Count, nameof(offset));

            var startIndex = source.Count - offset - 1;
            var endIndex = count > 0 && count < startIndex ? startIndex - count : 0;

            var arr2 = new T[2];
            var arr3 = new T[3];
            var arr4 = new T[4];

            for (var i = startIndex; i >= endIndex; i--)
            {
                arr2[0] = arr3[0] = arr4[0] = source[i];

                for (var j = i - 1; j >= endIndex; j--)
                {
                    arr2[1] = arr3[1] = arr4[1] = source[j];

                    if (match(arr2))
                        return [i, j];

                    for (var k = j - 1; k >= endIndex; k--)
                    {
                        arr3[2] = arr4[2] = source[k];

                        if (match(arr3))
                            return [i, j, k];

                        for (var l = k - 1; l >= endIndex; l--)
                        {
                            arr4[3] = source[l];
                            if (match(arr4))
                                return [i, j, k, l];
                        }
                    }

                }
            }

            return [];
        }

        #endregion

        public static T? FirstOrDefault<T>(this IList<T> source, Predicate<T> predicate, Predicate<T> preferredMatch, int startIndex = 0)
        {
            var endIndex = source.Count;
            var index = -1;

            for (var i = startIndex; i < endIndex; i++)
            {
                var item = source[i];

                if (!predicate(item))
                    continue;

                index = i;

                if (preferredMatch(item))
                    break;
            }

            return index != -1 ? source[index] : default;
        }

        public static IList<T> Combine<T>(this IList<T> source, IList<T> other)
        {
            var result = new List<T>(source.Count + other.Count);
            result.AddRange(source);
            result.AddRange(other);
            return result;
        }
                
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


        /// <summary>
        /// Inserts an element while maintaining the sorted order. If the element already exists, it will be omitted.
        /// </summary>
        /// <returns>returns true if the element was inserted, false otherwise</returns>
        public static bool InsertSorted<T>(this IList<T> elements, T element) where T : IComparable<T>
        {
            for (int i = 0; i < elements.Count; i++)
            {
                var compareResult = elements[i].CompareTo(element);

                // 1. Duplicate found
                if (compareResult == 0) // elements[i] == item
                    return false;

                // 2. We found where the new element fits
                if (compareResult > 0) // elements[i] > item
                {
                    elements.Insert(i, element);
                    return true;
                }
            }

            elements.Add(element);
            return true;
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
    }
}