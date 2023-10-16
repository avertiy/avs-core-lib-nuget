using System;

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
}