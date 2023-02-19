using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AVS.CoreLib.Extensions.Tasks;

/// <summary>
/// parallel wrapper over <see cref="IEnumerable{T}"/>
/// </summary>
public class Parallel<T,TResult> where T: notnull
{
    private readonly IEnumerable<T> _enumerable;

    private readonly Func<T, Task<TResult>> _job;

    public Parallel(IEnumerable<T> enumerable, Func<T, Task<TResult>> job)
    {
        _enumerable = enumerable;
        _job = job;
    }

    public async Task<List<TItem>> ToListAsync<TItem>(Func<T, TResult, IEnumerable<TItem>> selector)
    {
        var tasks = new Dictionary<T, Task<TResult>>();
        foreach (var key in _enumerable)
        {
            var task = _job(key);
            tasks.Add(key, task);
        }

        await Task.WhenAll(tasks.Values);

        var list = new List<TItem>();

        foreach (var kp in tasks)
        {
            var task = kp.Value;
            var result = task.Result;
            var items = selector(kp.Key, result);
            list.AddRange(items);
        }

        return list;
    }

    public async Task<List<TItem>> ToListAsync<TItem>(Func<TResult, IEnumerable<TItem>> selector)
    {
        var tasks = new Dictionary<T, Task<TResult>>();
        foreach (var key in _enumerable)
        {
            var task = _job(key);
            tasks.Add(key, task);
        }

        await Task.WhenAll(tasks.Values);

        var list = new List<TItem>();

        foreach (var kp in tasks)
        {
            var task = kp.Value;
            var result = task.Result;
            var items = selector(result);
            list.AddRange(items);
        }

        return list;
    }

    public TaskSet<T, TResult> CreateTaskSet()
    {
        return TaskSet.Create(_enumerable, _job);
    }

    public async Task<List<TResult>> ToListAsync()
    {
        var tasks = new Dictionary<T, Task<TResult>>();
        foreach (var key in _enumerable)
        {
            var task = _job(key);
            tasks.Add(key, task);
        }

        await Task.WhenAll(tasks.Values);

        var list = new List<TResult>();
        foreach (var kp in tasks)
        {
            var task = kp.Value;
            list.Add(task.Result);
        }
        return list;
    }

    public async Task<Dictionary<T, TResult>> GetResults()
    {
        var tasks = new Dictionary<T, Task<TResult>>();
        foreach (var key in _enumerable)
        {
            var task = _job(key);
            tasks.Add(key, task);
        }

        await Task.WhenAll(tasks.Values);

        var dict = new Dictionary<T, TResult>();
        foreach (var kp in tasks)
        {
            var task = kp.Value;
            dict.Add(kp.Key, task.Result);
        }
        return dict;
    }
}


public static class ParallelExtensions
{
    /// <summary>
    /// returns <see cref="Parallel{T, TResult}"/> wrapper
    /// </summary>
    public static Parallel<T, TResult> AsParallel<T, TResult>(this IEnumerable<T> enumerable, Func<T, Task<TResult>> job) where T : notnull
    {
        return new Parallel<T, TResult>(enumerable, job);
    }

    /// <summary>
    /// for each item executes a job, running tasks in parallel, then combine results to list
    /// </summary>
    public static async Task<List<TItem>> AsParallelToList<T, TResult, TItem>(this IEnumerable<T> enumerable,
        Func<T, Task<TResult>> job,
        Func<T, TResult, IEnumerable<TItem>> selector) where T : notnull
    {
        return await new Parallel<T, TResult>(enumerable, job).ToListAsync(selector);
    }

    /// <summary>
    /// for each item executes a job, running tasks in parallel, then combine results to list
    /// </summary>
    public static async Task<List<TItem>> AsParallelToList<T, TResult, TItem>(this IEnumerable<T> enumerable,
        Func<T, Task<TResult>> job,
        Func<TResult, IEnumerable<TItem>> selector) where T : notnull
    {
        return await new Parallel<T, TResult>(enumerable, job).ToListAsync(selector);
    }

    /// <summary>
    /// for each item executes a job, running tasks in parallel, returns results as list
    /// </summary>
    public static async Task<List<TResult>> AsParallelToList<T, TResult>(this IEnumerable<T> enumerable,
        Func<T, Task<TResult>> job) where T : notnull
    {
        return await new Parallel<T, TResult>(enumerable, job).ToListAsync();
    }

    /// <summary>
    /// for each item executes a job, running tasks in parallel, returns results as list
    /// </summary>
    public static async Task<Dictionary<T, TResult>> AsParallelToDictionary<T, TResult>(this IEnumerable<T> enumerable,
        Func<T, Task<TResult>> job) where T : notnull
    {
        return await new Parallel<T, TResult>(enumerable, job).GetResults();
    }

}