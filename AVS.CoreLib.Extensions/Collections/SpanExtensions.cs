using System;
using System.Text;

namespace AVS.CoreLib.Extensions.Collections
{
    /// <summary>
    /// Provides various extension methods for Span{T} to enhance its functionality
    /// </summary>
    public static class SpanExtensions
    {
        /// <summary>
        /// Determines whether all elements of a sequence (span) satisfy a condition.
        /// </summary>
        public static bool All<T>(this in Span<T> span, Func<T, bool> predicate, int startIndex = 0)
        {
            for (var i = startIndex; i < span.Length; i++)
            {
                if (!predicate(span[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether any element of a sequence (span) satisfy a condition.
        /// </summary>
        public static bool Any<T>(this in Span<T> span, Func<T, bool> predicate, int startIndex = 0)
        {
            for (var i = startIndex; i < span.Length; i++)
            {
                if (predicate(span[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether any element of a sequence (span) satisfy a condition.
        /// </summary>
        public static bool Any<T, TArg>(this in Span<T> span,TArg arg, Func<T,TArg, bool> predicate, int startIndex = 0)
        {
            for (var i = startIndex; i < span.Length; i++)
            {
                if (predicate(span[i], arg))
                    return true;
            }
            return false;
        }

        

        /// <summary>
        /// 
        /// </summary>
        public static int FindIndex<T, TArg>(this in Span<T> span, TArg argument, Func<T, TArg, bool> predicate, int startIndex = 0)
        {
            for (var i = startIndex; i < span.Length; i++)
            {
                if (predicate(span[i], argument))
                    return i;
            }
            return -1;
        }

        public static int FindIndex<T>(this in Span<T> span, Func<T, bool> predicate, int startIndex = 0)
        {
            for (var i = startIndex; i < span.Length; i++)
            {
                if (predicate(span[i]))
                    return i;
            }
            return -1;
        }

        public static int FindIndex<T1,T2>(this in Span<T1> span, Span<T2> span2, Func<T1, T2, bool> predicate)
        {
            var count = Math.Min(span.Length, span2.Length);
            for (var i = 0; i < count; i++)
            {
                if (predicate(span[i], span2[i]))
                    return i;
            }

            return -1;
        }

        public static int FindIndex<T1, T2, TArg>(this in Span<T1> span, Span<T2> span2, TArg arg, Func<(T1, T2), TArg, bool> predicate, int startIndex = 0)
        {
            var count = Math.Min(span.Length, span2.Length);
            for (var i = startIndex; i < count; i++)
            {
                if (predicate((span[i], span2[i]), arg))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Finds minimal element
        /// </summary>
        public static int Min(this in Span<int> span, int count, int startIndex = 0)
        {
            var min = span[startIndex];
            var n = Math.Min(count, span.Length);
            for (var i = startIndex + 1; i < n; i++)
            {
                if (span[i] < min)
                    min = span[i];
            }
            return min;
        }

        /// <summary>
        /// Finds maximal element
        /// </summary>
        public static int Max(this in Span<int> span, int count, int startIndex = 0)
        {
            var max = span[startIndex];
            var n = Math.Min(count, span.Length);
            for (var i = startIndex + 1; i < n; i++)
            {
                if (span[i] > max)
                    max = span[i];
            }
            return max;
        }

        /// <summary>
        /// Joins elements into string, analogue of string.Join(",", arr);
        /// </summary>
        public static string Stringify(this in Span<int> span, int count, int startIndex = 0, string separator = ",")
        {
            if (count == 0)
                return string.Empty;

            if (count == 1)
                return span[startIndex].ToString();

            var n = Math.Min(count, span.Length);
            var sb = new StringBuilder(n * 10);

            for (var i = startIndex + 1; i < n; i++)
            {
                sb.Append(span[i]);
                if (i +1 <n)
                    sb.Append(separator);
            }

            return sb.ToString();
        }
    }
}