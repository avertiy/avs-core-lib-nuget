using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.Extensions.Tasks;

public class TaskRunner<TResult>
{
    private readonly Delegate _job;
    /// <summary>
    /// delay (timeout) in milliseconds
    /// </summary>
    public int Delay { get; set; }

    [DebuggerStepThrough]
    public TaskRunner(Delegate job)
    {
        this._job = job;
    }

    [DebuggerStepThrough]
    public TaskRunner<TResult> WithDelay(int delayInMilliseconds)
    {
        Delay = delayInMilliseconds;
        return this;
    }
    
    public Task<TResult> Start<T>(T arg)
    {
        var fn = (Func<T, Task<TResult>>)_job;
        return fn(arg);
    }

    public Dictionary<T, Task<TResult>> StartAll<T>(IEnumerable<T> args)
    {
        var fn = (Func<T, Task<TResult>>)_job;
        var tasks = new Dictionary<T, Task<TResult>>();
        foreach (var arg in args)
        {
            var task = fn(arg);
            tasks.Add(arg, task);
            Sleep(Delay);
        }

        return tasks;
    }

    public async Task<TaskResults<T,TResult>> Execute<T>(IEnumerable<T> args)
    {
        var tasks = StartAll(args);
        await Task.WhenAll(tasks.Values);

        var results = new TaskResults<T,TResult>(tasks.Count);
        foreach(var kp in tasks)
        {
            var res = kp.Value.Result;
            results.Add(kp.Key, res);            
        }

        return results;
    }

    private static void Sleep(int delay)
    {
        if (delay > 0)
            Thread.Sleep(delay);
    }
}

public static class TaskRunner
{
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TaskRunner<TResult> Create<T, TResult>(Func<T, Task<TResult>> job, int delay = 0)
    {
        return new TaskRunner<TResult>(job) { Delay = delay };
    }

    public static async Task<TaskResults<T,TResult>> ExecuteOne<T,TResult>(this TaskRunner<TResult> runner, T arg)
    {
        var res = await runner.Start(arg);
        var results = new TaskResults<T, TResult>
        {
            { arg, res }
        };
        return results;
    }

    /// <summary>
    /// alias for RunAll
    /// </summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TaskResults<T, TResult>> RunAll<T, TResult>(this TaskRunner<TResult> runner, IEnumerable<T> args)
    {
        return runner.Execute(args);
    }

    [DebuggerStepThrough]
    public static Task<TaskResults<T, TResult>> RunAll<T, TResult>(this TaskRunner<TResult> runner, params T[] args)
    {
        return args.Length switch
        {
            0 => Task.FromResult(new TaskResults<T, TResult>()),
            1 => runner.ExecuteOne(args[0]),
            _ => runner.Execute(args),
        };
    }

    [DebuggerStepThrough]
    public static Task<TaskResults<T, TResult>> RunAll<T, TResult>(this TaskRunner<TResult> runner, IList<T> args)
    {
        return args.Count switch
        {
            0 => Task.FromResult(new TaskResults<T, TResult>()),
            1 => runner.ExecuteOne(args[0]),
            _ => runner.Execute(args),
        };
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<TOutput> RunAll<T, TResult, TOutput>(this TaskRunner<TResult> runner, IEnumerable<T> args, Func<TaskResults<T, TResult>, TOutput> selector)
    {
        return selector(await runner.Execute(args));
    }
}