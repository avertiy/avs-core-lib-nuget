using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Guards;

public static class GuardAgainstExtensions
{
    public static void NullOrEmpty(this IAgainstGuardClause guardClause, string? param, string name = "argument")
    {
        if (string.IsNullOrEmpty(param))
            throw new ArgumentNullException($"{name} must be not null or empty");
    }

    public static void NullOrEmpty(this IAgainstGuardClause guardClause, string? param, bool allowNull, string name = "argument")
    {
        if (string.IsNullOrEmpty(param) && !allowNull)
            throw new ArgumentNullException($"{name} must be not null or empty");
    }

    public static void NullOrEmpty<T>(this IAgainstGuardClause guardClause, T[]? arr, string? message = null)
    {
        if (arr == null || arr.Length == 0)
            throw new ArgumentNullException(message ?? $"Arg {typeof(T).Name}[] must be neither null neither empty");
    }

    public static void Empty<T>(this IAgainstGuardClause guardClause, T[] arr, string? message = null)
    {
        if (arr.Length == 0)
            throw new ArgumentNullException(message ?? $"Arg {typeof(T).Name}[] must be not empty");
    }

    public static void Empty<T>(this IAgainstGuardClause guardClause, IList<T> list, string? message = null)
    {
        if (list.Count == 0)
            throw new ArgumentNullException(message ?? $"Arg IList<{typeof(T).Name}> must be not empty");
    }

    public static void Empty<TKey,T>(this IAgainstGuardClause guardClause, IDictionary<TKey,T> dict, string? message = null)
    {
        if (dict.Count == 0)
            throw new ArgumentNullException(message ?? $"Arg IDictionary<{typeof(TKey).Name},{typeof(T).Name}> must be not empty");
    }

    public static void Null(this IAgainstGuardClause guardClause, object? arg, string? message = null)
    {
        if (arg == null)
            throw new ArgumentNullException(message ?? $"must be not null");
    }

    public static void Null(this IAgainstGuardClause guardClause, object? param, bool allowNull, string name = "argument")
    {
        if (param == null && !allowNull)
            throw new ArgumentNullException($"{name} must be not null");
    }

    public static void DateTimeMin(this IAgainstGuardClause guardClause, DateTime param, string name = "argument")
    {
        if (param == DateTime.MinValue)
            throw new ArgumentNullException($"{name} must be not a DateTime.MinValue");
    }
}