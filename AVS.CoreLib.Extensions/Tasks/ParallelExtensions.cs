using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVS.CoreLib.Extensions.Tasks;

public static class ParallelExtensions
{
    public static TResult Pipe<T, TResult>(this T value, Func<T, TResult> transform)
    {
        return transform(value);
    }

    public static T Let<T>(this T value, Action<T> action)
    {
        action(value);
        return value;
    }

    /// <summary>
    /// returns <see cref="ParallelJobs{T,TResult}"/> wrapper
    /// </summary>
    public static ParallelJobs<T, TResult> AsParallelJobs<T, TResult>(this IEnumerable<T> enumerable, Func<T, Task<TResult>> job) where T : notnull
    {
        return new ParallelJobs<T, TResult>(enumerable, job);
    }

    /// <summary>
    /// for each item creates job (task), executes them in parallel, then combine all results to single list of items
    /// </summary>
    public static Task<List<TItem>> ParallelFetchItems<T, TResult, TItem>(this IEnumerable<T> enumerable,
        Func<T, Task<TResult>> job,
        Func<T, TResult, IEnumerable<TItem>> selector) where T : notnull
    {
        return new ParallelJobs<T, TResult>(enumerable, job).FetchItemsAsync(selector);
    }

    /// <summary>
    /// for each item creates job (task), executes them in parallel, then combine all results to single list of items
    /// </summary>
    public static Task<List<TItem>> ParallelFetchItems<T, TResult, TItem>(this IEnumerable<T> enumerable,
        Func<T, Task<TResult>> job,
        Func<TResult, IEnumerable<TItem>> selector) where T : notnull
    {
        return new ParallelJobs<T, TResult>(enumerable, job).FetchItemsAsync(selector);
    }

    /// <summary>
    /// for each item creates job (task), executes them in in parallel, then returns results <see cref="List{TResult}"/>
    /// </summary>
    public static Task<List<TResult>> ParallelFetch<T, TResult>(this IEnumerable<T> enumerable,
        Func<T, Task<TResult>> job) where T : notnull
    {
        return new ParallelJobs<T, TResult>(enumerable, job).FetchAsync();
    }

    /// <summary>
    /// for each item creates job (task), executes them in in parallel, then returns results <see cref="Dictionary{T,TResults}"/> 
    /// </summary>
    public static Task<Dictionary<T, TResult>> ParallelFetchAsDictionary<T, TResult>(this IEnumerable<T> enumerable,
        Func<T, Task<TResult>> job) where T : notnull
    {
        return new ParallelJobs<T, TResult>(enumerable, job).FetchAsDictionaryAsync();
    }
}