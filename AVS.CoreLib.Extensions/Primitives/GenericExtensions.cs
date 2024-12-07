using System;
using System.Collections.Generic;
using AVS.CoreLib.Extensions.Enums;

namespace AVS.CoreLib.Extensions;

public static class GenericExtensions
{
    public static T Let<T>(this T value, Action<T> action)
    {
        action(value);
        return value;
    }

    public static TResult Pipe<T, TResult>(this T value, Func<T, TResult> transform)
    {
        return transform(value);
    }

    /// <summary>
    /// format enum value in upper case
    /// </summary>
    public static string ToUpperString<T>(this T value, string format = "G") where T : Enum
    {
        return value.ToString(format).ToUpper();
    }

    /// <summary>
    /// format enum value in lower case
    /// </summary>
    public static string ToLowerString<T>(this T value, string format = "G") where T : Enum
    {
        return value.ToString(format).ToLower();
    }

    /// <summary>
    /// format enum value in lower case
    /// </summary>
    public static string ToString<T>(this T value, StringCase @case, string format = "G") where T : Enum
    {
        return value.ToString(format).FormatString(@case);
    }

    public static bool IsNullOrEmpty<T>(this IList<T>? source)
    {
        return source == null || source.Count == 0;
    }
}