using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Extensions.Collections;

public static class ListSpecialExtensions
{
    #region Combinations & Subsets
    /// <summary>
    /// Generates combinations (subsets) without repetition
    /// <code>
    /// [1, 2, 3, 4].Combinations(2,3); => [1,2],[1,3],[1,4],[2,3],[2,4],[3,4], [1,2,3], [1,2,4], [1,3,4], [2,3,4]
    /// </code>
    /// <seealso cref="SliceExtensions.Slice{T}"/>
    /// </summary>
    public static IEnumerable<T[]> Combinations<T>(this IList<T> source, int minLength = 0, int maxLength = 0, int startIndex = 0)
    {
        Guard.MustBe.WithinRange(minLength, (0, source.Count));
        Guard.MustBe.WithinRange(maxLength, (minLength, source.Count));
        Guard.Against.Null(source);

        var minSize = minLength > 1 ? minLength : 2;

        if (minSize == source.Count)
        {
            yield return source.ToArray();
            yield break;
        }

        var maxSize = maxLength > 1 ? maxLength : source.Count - 1;

        for (var size = minSize; size <= maxSize; size++)
        {
            foreach (var combo in source.GetCombinations(size, startIndex))
            {
                yield return combo;
            }
        }
    }

    public static IEnumerable<TResult> Combinations<T, TResult>(this IList<T> source,
        Func<T[], TResult> selector,
        int minLength = 0,
        int maxLength = 0,
        int startIndex = 0)
    {
        Guard.Against.Null(source);
        Guard.Against.Null(selector);
        Guard.MustBe.WithinRange(minLength, (0, source.Count));
        Guard.MustBe.WithinRange(maxLength, (minLength, source.Count));

        var minSize = minLength > 1 ? minLength : 2;

        if (minSize == source.Count)
        {
            yield return selector(source.ToArray());
            yield break;
        }

        var maxSize = maxLength > 1 ? maxLength : source.Count - 1;

        for (var size = minSize; size <= maxSize; size++)
        {
            foreach (var combo in source.GetCombinations(size, startIndex))
            {
                yield return selector(combo);
            }
        }
    }

    /// <summary>
    /// Generates combinations (subsets) without repetition
    /// <code>
    /// [1, 2, 3, 4, 5].Combinations(3); => [1,2,3], [1,2,4], [1,2,5], [1,3,4], [1,3,5], [1,4,5], [2,3,4], [2,3,5], [2,4,5], [3,4,5]
    /// </code>
    /// </summary>
    private static IEnumerable<T[]> GetCombinations<T>(this IList<T> source, int n, int startIndex)
    {
        if (n <= 0 || startIndex < 0 || startIndex >= source.Count || n > source.Count - startIndex)
            yield break;

        var indices = Enumerable.Range(startIndex, n).ToArray();
        var max = source.Count;

        while (true)
        {
            yield return indices.Select(i => source[i]).ToArray();

            // Move rightmost index that can be incremented
            int i;
            for (i = n - 1; i >= 0; i--)
            {
                if (indices[i] >= max - n + i)
                    continue;

                indices[i]++;

                for (var j = i + 1; j < n; j++)
                    indices[j] = indices[j - 1] + 1;

                break;
            }

            if (i < 0)
                yield break;
        }
    }

    

    /// <summary>
    /// Finds a combination of a given size.
    /// Uses combinatorics to generate subsets
    /// <code>
    /// [1, 2, 1, 2, 3].FindCombination(x=> x.Sum() == 4, 3); => [1,2,1]
    /// </code>
    /// </summary>
    public static T[] FindCombination<T>(this IList<T> source, Func<T[], bool> match, int n, int startIndex = 0, int count = 0)
    {
        if (n <= 0 || startIndex < 0 || startIndex >= source.Count || n > source.Count - startIndex)
            return [];

        var indices = Enumerable.Range(startIndex, n).ToArray();
        var max = source.Count;
        var counter = count > 0 ? count : 10_000;// reasonable limit 
        while (counter-- > 0)
        {
            var subset = source.ElementsAt(indices);

            if (match(subset))
                return subset;

            // Move rightmost index that can be incremented
            int i;
            for (i = n - 1; i >= 0; i--)
            {
                if (indices[i] >= max - n + i)
                    continue;

                indices[i]++;

                for (var j = i + 1; j < n; j++)
                    indices[j] = indices[j - 1] + 1;

                break;
            }

            if (i < 0)
                break;
        }

        return [];
    }

    /// <summary>
    /// Finds a combination of a given size in range [from;to]
    /// <code>
    /// [1, 2, 1, 2, 3].FindCombination(x=> x.Sum() == 4, (2,3)); => possible matches: [1,3], [2,2], [1,2,1] => [1,3] (longer combinations will be iterated after shorter ones)
    /// </code>
    /// </summary>
    public static T[] FindCombination<T>(this IList<T> source, Func<T[], bool> match, (int from, int to) size, int startIndex = 0)
    {
        Guard.MustBe.ValidRange(size, 0, source.Count - startIndex);

        for (var i = size.from; i < size.to; i++)
        {
            var comb = source.FindCombination(match, i, startIndex);

            if (comb.Length > 0)
                return comb;
        }

        return [];
    }

    /// <summary>
    /// Helps to find a subset of a few (2-4) elements.
    /// Fast, targeted search, optimized for performance and memory usage (uses nested loops to iterate through, no recursion).
    /// <code>
    /// [1, 2, 1, 2, 3].FindSubset(x=> x.Sum() == 4); => possible matches: [1,3], [1,2,1], [2,2] => [1,3] (will pick the 1st one)
    /// </code>
    /// </summary>
    public static T[] FindSubset<T>(this IList<T> source, Func<T[], bool> match, int startIndex = 0, int lookupLength = 0)
    {
        Guard.MustBe.WithinRange(startIndex, 0, source.Count, nameof(startIndex));
        Guard.MustBe.GreaterThanOrEqual(lookupLength, 0);

        var n = lookupLength > 0 && lookupLength < source.Count ? lookupLength : source.Count;
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
                    return arr2;

                for (var k = j + 1; k < n; k++)
                {
                    arr3[2] = arr4[2] = source[k];

                    if (match(arr3))
                        return arr3;

                    for (var l = k + 1; l < n; l++)
                    {
                        arr4[3] = source[l];
                        if (match(arr4))
                            return arr4;
                    }
                }

            }
        }

        return [];
    }
    #endregion

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

    /// <summary>
    /// Recursive search for the elements whose sum will give the required qty
    /// </summary>
    //[Obsolete("Not sure it has usages")]
    public static int[] FindIndexes(this IList<decimal> source, decimal qty, int startIndex = 0, int limitRecursion = 0)
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

            var arr = FindIndexes(source, rest, i + 1, limitRecursion - 1);

            if (arr.Any())
                return arr.Insert(i);
        }

        return Array.Empty<int>();
    }

    public static IList<T> Shrink<T>(this IList<T> items, Func<T, double> selector, double threshold = 0.0)
    {
        var avg = items.Average(selector);
        if (threshold <= 0)
            threshold = avg;
        return items.Where(i => selector(i) >= threshold).ToList();
    }
}