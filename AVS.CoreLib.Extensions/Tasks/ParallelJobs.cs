using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.Extensions.Tasks;

/// <summary>
/// parallel runner helps to run multiple tasks (jobs) and combining results into List or Dictionary
/// </summary>
public class ParallelJobs<T,TResult> where T: notnull
{
    /// <summary>
    /// delay (timeout) in milliseconds
    /// </summary>
    private int _delay;
    private readonly IEnumerable<T> _enumerable;

    private readonly Func<T, Task<TResult>> _job;

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

            if (task.Result != null)
                list.Add(task.Result);
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
            var items = selector(result);
            
            if (items == null)
                continue;

            list.AddRange(items);
        }

        return list;
    }
    
    public ParallelJobs<T, TResult> WithDelay(int delayInMilliseconds)
    {
        _delay = delayInMilliseconds;
        return this;
    }

    private void Delay()
    {
        if(_delay > 0)
            Thread.Sleep(_delay);
    }


    //public TaskSet<T, TResult> CreateTaskSet()
    //{
    //    return TaskSet.Create(_enumerable, _job);
    //}

}


public static class ParallelExtensions
{
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