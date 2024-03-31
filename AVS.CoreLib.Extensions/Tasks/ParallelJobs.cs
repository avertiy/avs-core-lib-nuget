using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.Extensions.Tasks;
/*
/// <summary>
/// parallel runner helps to run multiple tasks (jobs) and combining results into List or Dictionary
/// </summary>
public sealed class ParallelJobs<T,TResult> : IEnumerable<T>
    where T: notnull
{
    private readonly IEnumerable<T> _enumerable;
    private readonly Func<T, Task<TResult>> _job;
    private Func<TResult, string?>? _checkErrorFn = null;
    /// <summary>
    /// delay (timeout) in milliseconds
    /// </summary>
    public int Timeout { get; private set; }
    
    public TResult[]? AllResults { get; private set; }
    
    public Dictionary<T, string>? Errors { get; private set; } = null;

    public bool HasErrors => Errors != null && Errors.Any();

    public ParallelJobs(IEnumerable<T> enumerable, Func<T, Task<TResult>> job)
    {
        _enumerable = enumerable;
        _job = job;
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

    /// <summary>
    /// run 1 job
    /// </summary>
    public Task<TResult> Run(T arg)
    {
        return _job(arg);
    }

    public Dictionary<T, Task<TResult>> RunAll(CancellationToken ct = default)
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

        AllResults = new TResult[tasks.Count];

        return tasks;
    }

    internal bool ValidateResult(T key, TResult result)
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

    public IEnumerator<T> GetEnumerator()
    {
        return _enumerable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void Delay()
    {
        if (Timeout > 0)
            Thread.Sleep(Timeout);
    }    
}

public static class ParallelJobsExtensions
{
    /// <summary>
    /// execute jobs fetch data returning list of results <see cref="List{TResult}"/>
    /// in case error check <seealso cref="WithErrorCheck"/> not passed  i.e. has error the failed result is skipped
    /// you can access still it via <see cref="AllResults"/> property
    /// </summary>
    public static async Task<List<TResult>> FetchAsync<T, TResult>(this ParallelJobs<T, TResult> jobs, CancellationToken ct = default) where T : notnull
    {
        var tasks = jobs.RunAll(ct);
        await Task.WhenAll(tasks.Values).ConfigureAwait(false);

        var list = new List<TResult>(tasks.Count);        
        var i = 0;

        foreach (var kp in tasks)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = kp.Value;
            var result = task.Result;
            jobs.AllResults![i++] = result;

            if (jobs.ValidateResult(kp.Key, result))
                continue;

            if (result != null)
                list.Add(result);
        }

        return list;
    }

    /// <summary>
    /// Execute jobs and fetch data returning <see cref="Dictionary{T, TResult}"/>
    /// </summary>
    public static async Task<Dictionary<T, TResult>> FetchAsDictionaryAsync<T, TResult>(this ParallelJobs<T, TResult> jobs, CancellationToken ct = default) where T : notnull
    {
        var tasks = jobs.RunAll(ct);
        await Task.WhenAll(tasks.Values).ConfigureAwait(false);
        var dict = new Dictionary<T, TResult>();
        var i = 0;
        foreach (var kp in tasks)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = kp.Value;
            jobs.AllResults![i++] = task.Result;
            if (jobs.ValidateResult(kp.Key, task.Result))
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
    public static async Task<List<TItem>> FetchItemsAsync<T, TResult, TItem>(this ParallelJobs<T, TResult> jobs, Func<T, TResult, IEnumerable<TItem>> selector, CancellationToken ct = default) where T : notnull
    {
        var tasks = jobs.RunAll(ct);
        await Task.WhenAll(tasks.Values).ConfigureAwait(false);        
        var list = new List<TItem>();

        var i = 0;
        foreach (var kp in tasks)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = kp.Value;
            var result = task.Result;
            jobs.AllResults![i++] = result;

            if (jobs.ValidateResult(kp.Key, result))
                continue;

            var items = selector(kp.Key, result);

            if (items == null)
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
    public static async Task<List<TItem>> FetchItemsAsync<T, TResult,TItem>(this ParallelJobs<T, TResult> jobs, Func<TResult, IEnumerable<TItem>> selector, CancellationToken ct = default) where T : notnull
    {
        var tasks = jobs.RunAll(ct);
        await Task.WhenAll(tasks.Values).ConfigureAwait(false);

        var list = new List<TItem>();
        var i = 0;
        foreach (var kp in tasks)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = kp.Value;
            var result = task.Result;
            jobs.AllResults![i++] = result;

            if (jobs.ValidateResult(kp.Key, result))
                continue;

            var items = selector(result);

            if (items == null)
                continue;

            list.AddRange(items);
        }

        return list;
    }

    public static async Task<Dictionary<TKey, TItem>> FetchItemsAsync<T, TResult,TItem, TKey>(this ParallelJobs<T, TResult> jobs, Func<TResult, IEnumerable<TItem>> selector, Func<TItem, TKey> keySelector, CancellationToken ct = default) where T : notnull
    {
        var tasks = jobs.RunAll(ct);
        await Task.WhenAll(tasks.Values).ConfigureAwait(false);        
        //var hashSet = new HashSet<TItem>();
        var dict = new Dictionary<TKey, TItem>();

        var i = 0;
        foreach (var kp in tasks)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = kp.Value;
            var result = task.Result;
            jobs.AllResults![i++] = result;

            if (jobs.ValidateResult(kp.Key, result))
                continue;

            var items = selector(result);

            if (items == null)
                continue;

            foreach (var item in items)
            {
                var key = keySelector(item);
                if (dict.ContainsKey(key))
                    continue;

                dict.Add(key, item);
            }
        }

        return dict;
    }

}
*/