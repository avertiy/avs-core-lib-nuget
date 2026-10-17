using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Extensions.Collections
{
    /// <summary>
    /// Provides various extension methods to array functionality
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Inserts an element into a sorted array while maintaining the sorted order. If the element already exists in the array, it will be omitted.
        /// </summary>
        /// <returns>returns true if the element was inserted, false otherwise</returns>
        public static bool InsertSorted<T>(this T[] elements, T element) where T : IComparable<T>
        {
            for (int i = 0; i < elements.Length; i++)
            {
                var compareResult = elements[i].CompareTo(element);

                // 1. Duplicate found
                if (compareResult == 0) // elements[i] == item
                    return false;

                // 2. We found where the new element fits
                if (compareResult > 0) // elements[i] > item
                {
                    // Shift everything from 'i' to the right by 1
                    // (Since max length is 10, Array.Copy or a mini loop is extremely cheap)
                    Array.Copy(elements, i, elements, i + 1, elements.Length - i - 1);
                    elements[i] = element;
                    return true;
                }

                // 3. Reached the end
                if (EqualityComparer<T>.Default.Equals(elements[i], default(T)))
                {
                    elements[i] = element;
                    return true;
                }
            }

            return false; // Array is full
        }

        /// <summary>
        /// Converts the array to a string representation.
        /// </summary>
        public static string AsString<T>(this T[] arr, string separator = ",", bool wrapInBrackets = true)
        {
            return wrapInBrackets
                ? $"[{string.Join(separator, arr)}]"
                : string.Join(separator, arr);
        }
    }
}