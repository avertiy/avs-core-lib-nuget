using System;
using System.Collections.Generic;

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



    #endregion
}