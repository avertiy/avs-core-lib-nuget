using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Guards;

public static class GuardAgainstExtensions
{
    #region Null
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
    #endregion

    #region NullOrEmpty
    public static void NullOrEmpty(this IAgainstGuardClause guardClause, string? param, string? message = null)
    {
        if (string.IsNullOrEmpty(param))
            throw new ArgumentNullException(message ?? "must be not null nor empty");
    }

    public static void NullOrEmpty(this IAgainstGuardClause guardClause, string? param, bool allowNull, string? message = null)
    {
        if (string.IsNullOrEmpty(param) && !allowNull)
            throw new ArgumentNullException(message ?? $"must be not null mor empty");
    }

    public static void NullOrEmpty<T>(this IAgainstGuardClause guardClause, T[]? arr, string? message = null)
    {
        if (arr == null || arr.Length == 0)
            throw new ArgumentNullException(message ?? $"Arg {typeof(T).Name}[] must be neither null neither empty");
    }
    #endregion

    #region Zero
    public static void Zero(this IAgainstGuardClause guardClause, int param, string? message = null)
    {
        if (param == 0)
            throw new ArgumentNullException(message ?? "must be not 0");
    }

    public static void Zero(this IAgainstGuardClause guardClause, long param, string? message = null)
    {
        if (param == 0)
            throw new ArgumentNullException(message ?? "must be not 0");
    }

    public static void Zero(this IAgainstGuardClause guardClause, decimal param, string? message = null)
    {
        if (param == 0)
            throw new ArgumentNullException(message ?? "must be not 0");
    }

    public static void Zero(this IAgainstGuardClause guardClause, double param, string? message = null)
    {
        if (param == 0)
            throw new ArgumentNullException(message ?? "must be not 0");
    }

    #endregion

    #region NullOrZero
    public static void NullOrZero(this IAgainstGuardClause guardClause, int? param, string? message = null)
    {
        if (param == null || param.Value == 0)
            throw new ArgumentNullException(message ?? "must be not null nor 0");
    }

    public static void NullOrZero(this IAgainstGuardClause guardClause, long? param, string? message = null)
    {
        if (param == null || param.Value == 0)
            throw new ArgumentNullException(message ?? "must be not null nor 0");
    }

    public static void NullOrZero(this IAgainstGuardClause guardClause, decimal? param, string? message = null)
    {
        if (param == null || param.Value == 0)
            throw new ArgumentNullException(message ?? "must be not null nor 0");
    }

    public static void NullOrZero(this IAgainstGuardClause guardClause, double? param, string? message = null)
    {
        if (param == null || param.Value == 0)
            throw new ArgumentNullException(message ?? "must be not null nor 0");
    }
    #endregion

    #region Empty
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

    public static void Empty<TKey, T>(this IAgainstGuardClause guardClause, IDictionary<TKey, T> dict, string? message = null)
    {
        if (dict.Count == 0)
            throw new ArgumentNullException(message ?? $"Arg IDictionary<{typeof(TKey).Name},{typeof(T).Name}> must be not empty");
    }
    #endregion

    public static void MinValue(this IAgainstGuardClause guardClause, DateTime param, string? message = null)
    {
        if (param == DateTime.MinValue)
            throw new ArgumentNullException(message ?? $"must be not min value {DateTime.MinValue:g}");
    }

    public static void MinValue(this IAgainstGuardClause guardClause, int param, string? message = null)
    {
        if (param == int.MinValue)
            throw new ArgumentNullException(message ?? $"must be not min value {int.MinValue}");
    }
}