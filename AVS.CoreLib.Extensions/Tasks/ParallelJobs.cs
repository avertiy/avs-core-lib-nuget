using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.Extensions.Stringify;

namespace AVS.CoreLib.Extensions.Tasks;

/// <summary>
/// parallel runner helps to run multiple tasks (jobs) and combining results into List or Dictionary
/// </summary>
public sealed class ParallelJobs<T,TResult> : IEnumerable<T>
    where T: notnull
{
    /// <summary>
    /// delay (timeout) in milliseconds
    /// </summary>
    public int Timeout { get; private set; }
    private readonly IEnumerable<T> _enumerable;

    private readonly Func<T, Task<TResult>> _job;

    private Func<TResult, string?>? _checkErrorFn = null;
    public Dictionary<T, string>? Errors { get; private set; } = null;
    public bool HasErrors => Errors != null && Errors.Any();

    public ParallelJobs(IEnumerable<T> enumerable, Func<T, Task<TResult>> job)
    {
        _enumerable = enumerable;
        _job = job;
    }

    /// <summary>
    /// execute jobs and fetch data returning <see cref="List{TResult}"/>
    /// </summary>
    public async Task<List<TResult>> FetchAsync(CancellationToken ct = default)
    {
        var tasks = new Dictionary<T, Task<TResult>>();
        foreach (var key in _enumerable)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = _job(key);
            tasks.Add(key, task);
            Delay();
        }

        await Task.WhenAll(tasks.Values).ConfigureAwait(false);

        var list = new List<TResult>();

        foreach (var kp in tasks)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = kp.Value;
            var result = task.Result;

            if (HasError(kp.Key, result))
                continue;

            if (result != null)
                list.Add(result);
        }
        return list;
    }

    /// <summary>
    /// Execute jobs and fetch data returning <see cref="Dictionary{T, TResult}"/>
    /// </summary>
    public async Task<Dictionary<T, TResult>> FetchAsDictionaryAsync(CancellationToken ct = default)
    {
        var tasks = new Dictionary<T, Task<TResult>>();
        foreach (var key in _enumerable)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = _job(key);
            tasks.Add(key, task);
            Delay();
        }

        await Task.WhenAll(tasks.Values).ConfigureAwait(false);

        var dict = new Dictionary<T, TResult>();

        foreach (var kp in tasks)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = kp.Value;

            if (HasError(kp.Key, task.Result))
                continue;

            dict.Add(kp.Key, task.Result);
        }
        return dict;
    }

    /// <summary>
    /// Execute jobs and fetch data returning items (<see cref="TItem"/> ) combined into single list of items <see cref="List{TItem}"/>
    /// </summary>
    /// <remarks>
    /// It is supposed that <see cref="TResult"/> carries array or list of items.
    /// The items are picked from result by <see cref="selector"/>
    /// Due to multiple jobs are executed they will produce list of results that means list of lists of items
    /// Instead of returning list of lists, items combined into a single list.
    /// </remarks>
    public async Task<List<TItem>> FetchItemsAsync<TItem>(Func<T, TResult, IEnumerable<TItem>> selector, CancellationToken ct = default)
    {
        var tasks = new Dictionary<T, Task<TResult>>();
        foreach (var key in _enumerable)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = _job(key);
            tasks.Add(key, task);
            Delay();
        }

        await Task.WhenAll(tasks.Values).ConfigureAwait(false);

        var list = new List<TItem>();

        foreach (var kp in tasks)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = kp.Value;
            var result = task.Result;

            if (HasError(kp.Key, result))
                continue;

            var items = selector(kp.Key, result);
            
            if(items == null)
                continue;

            list.AddRange(items);
        }

        return list;
    }

    /// <summary>
    /// Execute jobs and fetch data returning items (<see cref="TItem"/> ) combined into single list of items  <see cref="List{TItem}"/>
    /// </summary>
    /// <remarks>
    /// It is supposed that <see cref="TResult"/> carries array or list of items.
    /// The items are picked from result by <see cref="selector"/>
    /// Due to multiple jobs are executed they will produce list of results that means list of lists of items
    /// Instead of returning list of lists, items combined into a single list.
    /// </remarks>
    public async Task<List<TItem>> FetchItemsAsync<TItem>(Func<TResult, IEnumerable<TItem>> selector, CancellationToken ct = default)
    {
        var tasks = new Dictionary<T, Task<TResult>>();
        foreach (var key in _enumerable)
        {
            if(ct.IsCancellationRequested)
                break;

            var task = _job(key);
            tasks.Add(key, task);
            Delay();
        }

        await Task.WhenAll(tasks.Values).ConfigureAwait(false);

        var list = new List<TItem>();

        foreach (var kp in tasks)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = kp.Value;
            var result = task.Result;

            if (HasError(kp.Key, result))
                continue;

            var items = selector(result);
            
            if (items == null)
                continue;

            list.AddRange(items);
        }

        return list;
    }

    private bool HasError(T key, TResult result)
    {
        if (_checkErrorFn == null)
            return false;

        var err = _checkErrorFn(result);
        if (err == null)
            return false;

        Errors ??= new Dictionary<T, string>();
        Errors.Add(key, err);
        return true;
    }

    public ParallelJobs<T, TResult> WithDelay(int delayInMilliseconds)
    {
        Timeout = delayInMilliseconds;
        return this;
    }

    /// <summary>
    /// you can check whether <see cref="TResult"/> contains error, if any the result is ignored
    /// and the error is recorded separately <see cref="Errors"/> 
    /// </summary>
    public ParallelJobs<T, TResult> WithErrorCheck(Func<TResult, string?> checkFn)
    {
        _checkErrorFn = checkFn;
        return this;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _enumerable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Task<TResult> Execute(T arg)
    {
        return _job(arg);
    }

    private void Delay()
    {
        if (Timeout > 0)
            Thread.Sleep(Timeout);
    }

    /// <summary>
    /// return <see cref="Errors"/> joined into single string
    /// if <see cref="distinct"/> and all errors are the same it return comma separated keys and error message
    /// otherwise <see cref="Errors"/> are stringified with <seealso cref="StringifyExtensions"/>
    /// </summary>
    public string? GetErrorsCombined(bool distinct = true)
    {
        if (Errors == null || Errors.Count == 0)
            return null;

        if (!distinct) 
            return Errors.Stringify();

        var distinctErrors = Errors.Values.Distinct().ToArray();

        return distinctErrors.Length == 1 ? 
            $"{string.Join(",", Errors.Keys)}: {distinctErrors[0]}" :
            Errors.Stringify();
    }
}

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