using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.Extensions.Enums;

namespace AVS.CoreLib.Guards;

public static class GuardArrayExtensions
{
    #region Items guards
    /// <summary>
    /// check array index, it should be in range [0,arr.Length] 
    /// </summary>
    public static void CheckIndex<T>(this IArrayGuardClause guardClause, int index, T[] arr, string? message = null)
    {
        if (index < 0 || index >= arr.Length)
            throw new ArgumentOutOfRangeException(message ?? $"[{index}] should be within range [0; {arr.Length - 1}]");
    }

    public static void Length<T>(this IArrayGuardClause guardClause, T[] arr, int length, string? message = null)
    {
        if (arr.Length != length)
            throw new ArgumentOutOfRangeException(message ?? $"{length} element(s) expected");
    }


    public static void MinLength<T>(this IArrayGuardClause guardClause, T[] arr, int minLength, string? message = null)
    {
        if (arr.Length < minLength)
            throw new ArgumentOutOfRangeException(message ?? $"at least {minLength} element(s) required");
    }



    public static void MaxLength<T>(this IArrayGuardClause guardClause, T[] arr, int maxLength, string? message = null)
    {
        if (arr.Length > maxLength)
            throw new ArgumentOutOfRangeException(message ?? $"Max {maxLength} element(s) allowed");
    }



    #endregion

    #region IList

    public static void CheckIndex<T>(this IArrayGuardClause guardClause, IList<T> list, int index, string name = "index")
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException($"{name} [{index}] must be positive number");

        if (index > list.Count)
            throw new ArgumentOutOfRangeException($"{name} [{index}] must not exceed {list.Count}");
    }

    public static void Length<T>(this IArrayGuardClause guardClause, IList<T> arr, int length, string? message = null)
    {
        if (arr.Count != length)
            throw new ArgumentOutOfRangeException(message ?? $"{length} element(s) expected");
    }

    public static void MinLength<T>(this IArrayGuardClause guardClause, IList<T> arr, int minLength, string? message = null)
    {
        if (arr.Count < minLength)
            throw new ArgumentOutOfRangeException(message ?? $"At least {minLength} element(s) required");
    }

    public static void MaxLength<T>(this IArrayGuardClause guardClause, IList<T> arr, int maxLength, string? message = null)
    {
        if (arr.Count > maxLength)
            throw new ArgumentOutOfRangeException(message ?? $"Max {maxLength} element(s) allowed");
    }

    public static void MustHaveAtLeast<T>(this IArrayGuardClause guardClause, IList<T> list, int itemsCount, string name = "list")
    {
        if (list == null)
            throw new ArgumentNullException($"{name} must be not null");

        if (list.Count < itemsCount)
            throw new ArgumentOutOfRangeException($"{name} must have at least #{itemsCount} items");
    }

    public static void MustBeSorted<T>(this IArrayGuardClause guardClause, IList<T> source, Sort sort, IComparer<T> comparer, int count = 0, string? message = null)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (source.Count <= 1 || sort == Sort.None)
            return;

        if (sort == Sort.Asc)
        {
            for (var i = 0; i < source.Count-1; i++)
            {
                if (comparer.Compare(source[i], source[i+1]) > 0)
                    throw new ArgumentException(message ?? "Source must be ordered ascending");

                count--;
                if (count == 0)
                    break;
            }
        }
        else
        {
            for (var i = 0; i < source.Count - 1; i++)
            {
                if (comparer.Compare(source[i], source[i + 1]) < 0)
                    throw new ArgumentException(message ?? "Source must be ordered descending");

                count--;
                if (count == 0)
                    break;
            }
        }
    }
    
    public static void MustBeDescending<T, TKey>(this IArrayGuardClause guardClause, IList<T> source, Func<T, TKey> selector, int count = 0, string? message = null) where TKey : IComparable<TKey>
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (source.Count <= 1)
            return;

        var previous = selector(source[0]);

        for (var i = 1; i < source.Count; i++)
        {
            var current = selector(source[i]);
            if (previous.CompareTo(current) < 0)
                throw new ArgumentException(message ?? "Source must be ordered descending");

            previous = current;

            count--;
            if (count == 0)
                break;

        }
    }

    public static void MustBeAscending<T, TKey>(this IArrayGuardClause guardClause, IList<T> source, Func<T, TKey> selector, int count = 0, string? message = null) where TKey : IComparable<TKey>
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (source.Count <= 1)
            return;

        var previous = selector(source[0]);

        for (var i = 1; i < source.Count; i++)
        {
            var current = selector(source[i]);
            if (current.CompareTo(previous) < 0)
                throw new ArgumentException(message ?? "Source must be ordered ascending");

            previous = current;

            count--;
            if (count == 0)
                break;
        }
    }

    public static void All<T>(this IArrayGuardClause guardClause, IList<T> source, Func<T,bool> predicate, string? message = null)
    {
        var index = source.FindIndex(x => !predicate(x), 0);
        if (index == -1)
            return;

        var errDetails = $"Element at index [{index}] does not satisfy predicate.";
        var error = message == null 
            ? $"All elements must satisfy the given predicate. {errDetails}"
            : $"{message} {errDetails}";
        
        throw new ArgumentException(error, nameof(source));
    }

    #endregion
}