using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.Extensions.Tasks;

/// <summary>
/// parallel job runner helps in running multiple tasks (jobs) in parallel with a configured delay
/// </summary>
public sealed class ParallelJobs<TKey, TResult> where TKey: notnull
{
    private readonly IEnumerable<TKey> _enumerable;
    private readonly Func<TKey, Task<TResult>> _job;
    private Func<TResult, string?>? _checkErrorFn = null;
    /// <summary>
    /// delay (timeout) in milliseconds
    /// </summary>
    public int Timeout { get; private set; }
    public string? Errors { get; private set; }

    [DebuggerStepThrough]
    public ParallelJobs(IEnumerable<TKey> enumerable, Func<TKey, Task<TResult>> job)
    {
        _enumerable = enumerable;
        _job = job;
    }

    [DebuggerStepThrough]
    public ParallelJobs<TKey, TResult> WithDelay(int delayInMilliseconds)
    {
        Timeout = delayInMilliseconds;
        return this;
    }

    /// <summary>
    /// you can check whether <see cref="TResult"/> contains error, if any the result is ignored
    /// and the error is recorded separately <see cref="Errors"/> 
    /// </summary>
    [DebuggerStepThrough]
    public ParallelJobs<TKey, TResult> WithErrorCheck(Func<TResult, string?> checkFn)
    {
        _checkErrorFn = checkFn;
        return this;
    }


    /// <summary>
    /// run 1 job
    /// </summary>
    public Task<TResult> Run(TKey arg)
    {
        return _job(arg);
    }

    [DebuggerStepThrough]
    public async Task<TOutput> RunAll<TOutput>(Func<TaskResults<TResult>,TOutput> func, CancellationToken ct = default)
    {
        var tasks = new Dictionary<TKey, Task<TResult>>();
        foreach (var key in _enumerable)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = _job(key);
            tasks.Add(key, task);
            Delay();
        }

        await Task.WhenAll(tasks.Values);
        var taskResults = CreateResults(tasks);        
        var result = func(taskResults);
        Errors = taskResults.HasErrors ? taskResults.GetCombinedErrors() : null;
        return result;
    }

    [DebuggerStepThrough]
    public async Task<TaskResults<TResult>> RunAll(CancellationToken ct = default)
    {
        var tasks = new Dictionary<TKey, Task<TResult>>();
        foreach (var key in _enumerable)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = _job(key);
            tasks.Add(key, task);
            Delay();
        }

        await Task.WhenAll(tasks.Values);
        var results = CreateResults(tasks);
        Errors = results.HasErrors ? results.GetCombinedErrors() : null;
        return results;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<List<TItem>> FetchItems<TItem>(Func<TResult, IEnumerable<TItem>> selector, CancellationToken ct = default)
    {
        return RunAll(x => x.PickItems(selector) ,ct);
    }

    /// <summary>
    /// unwrap Task results from Tasks
    /// </summary>
    private TaskResults<TResult> CreateResults(Dictionary<TKey, Task<TResult>> tasks)
    {
        var results = new TaskResults<TResult>(tasks.Count);        
        foreach (var kp in tasks)
        {
            var key = kp.Key;
            var task = kp.Value;
            var result = task.Result;

            if (!IsValidResult(result, out var error))
            {
                results.Add(key, result, error!);                
                continue;
            }

            results.Add(key, result);
        }
        
        return results;
    }

    private bool IsValidResult(TResult result, out string? error)
    {
        error = null;
        if (result == null)
            return false;

        if (_checkErrorFn == null)
            return true;

        error = _checkErrorFn(result);
        return error == null;
    }

    private void Delay()
    {
        if (Timeout > 0)
            Thread.Sleep(Timeout);
    }
}
