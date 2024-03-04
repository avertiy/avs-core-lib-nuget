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
    /// delay in milliseconds between tasks
    /// </summary>
    public int Delay { get; set; }
    /// <summary>
    /// timout in milliseconds
    /// </summary>
    public int Timeout { get; set; }

    /// <summary>
    /// Batch size limit when provided instructs runner to run tasks up to batch size limit, wait completion, then run the next portion of tasks
    /// </summary>
    public int BatchSize { get; set; }
    /// <summary>
    /// Batch timespan in milliseconds e.g. you have 50 requests and throughput 10 requests per minute
    /// </summary>
    public int BatchTimespan { get; set; }

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

    [DebuggerStepThrough]
    public TaskRunner<TResult> WithTimeout(int timeout)
    {
        Timeout = timeout;
        return this;
    }


    [DebuggerStepThrough]
    public TaskRunner<TResult> UseBatchMode(int batchSize, int batchDelay)
    {
        BatchSize = batchSize;
        BatchTimespan = batchDelay;
        return this;
    }

    public Task<TResult> Start<T>(T arg)
    {
        var fn = (Func<T, Task<TResult>>)_job;
        return fn(arg);
    }

    public async Task<TaskResults<T,TResult>> RunAll<T>(IEnumerable<T> args, CancellationToken ct = default) where T : notnull
    {
        if (BatchSize > 0)
            return await ExecuteInBatchMode(args, ct);

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

    private async Task<TaskResults<T, TResult>> ExecuteInBatchMode<T>(IEnumerable<T> args, CancellationToken ct) where T : notnull
    {
        var fn = (Func<T, Task<TResult>>)_job;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var queue = new Queue<(T key, Task<TResult> result)>(BatchSize+1);
        var results = new TaskResults<T, TResult>(BatchSize*2);
        
        var processQueue = async () => {
            while (queue.Count > 0)
            {
                if (Timeout > 0 && stopwatch.ElapsedMilliseconds > Timeout)
                    break;

                var (key, task) = queue.Dequeue();
                await task;
                results.Add(key, task.Result);
            }
        };

        var elapsed = 0;

        foreach (var arg in args)
        {
            if (ct.IsCancellationRequested)
                break;

            if (Timeout > 0 && stopwatch.ElapsedMilliseconds > Timeout)
                break;

            queue.Enqueue((arg, fn(arg)));
            Sleep(Delay);
            if (queue.Count == BatchSize)
            {
                await processQueue();
                var delay = BatchTimespan - (int)stopwatch.ElapsedMilliseconds - elapsed;
                Sleep(delay);
                elapsed =(int)stopwatch.ElapsedMilliseconds;
            }
        }
        
        await processQueue();

        return results;
    }

    private Dictionary<T, Task<TResult>> StartAll<T>(IEnumerable<T> args) where T : notnull
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
    public static TaskRunner<TResult> Create<T, TResult>(Func<T, Task<TResult>> job, int delay = 0, int timeout = 0)
    {
        return new TaskRunner<TResult>(job) { Delay = delay, Timeout = timeout };
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TaskRunner<TResult> Create<T, TResult>(Func<T, Task<TResult>> job, Options options)
    {
        return new TaskRunner<TResult>(job) { 
            Delay = options.Delay, 
            Timeout = options.Timeout, 
            BatchSize = options.BatchSize, 
            BatchTimespan = options.BatchTimespan };
    }

    public static async Task<TaskResults<T,TResult>> ExecuteOne<T,TResult>(this TaskRunner<TResult> runner, T arg) where T : notnull
    {
        var res = await runner.Start(arg);
        var results = new TaskResults<T, TResult>
        {
            { arg, res }
        };
        return results;
    }

    [DebuggerStepThrough]
    public static Task<TaskResults<T, TResult>> RunAll<T, TResult>(this TaskRunner<TResult> runner, params T[] args) where T : notnull
    {
        return args.Length switch
        {
            0 => Task.FromResult(new TaskResults<T, TResult>()),
            1 => runner.ExecuteOne(args[0]),
            _ => runner.RunAll(args),
        };
    }

    [DebuggerStepThrough]
    public static Task<TaskResults<T, TResult>> RunAll<T, TResult>(this TaskRunner<TResult> runner, IList<T> args, CancellationToken ct = default) where T : notnull
    {
        return args.Count switch
        {
            0 => Task.FromResult(new TaskResults<T, TResult>()),
            1 => runner.ExecuteOne(args[0]),
            _ => runner.RunAll(args, ct),
        };
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<TOutput> RunAll<T, TResult, TOutput>(this TaskRunner<TResult> runner, IEnumerable<T> args, Func<TaskResults<T, TResult>, TOutput> selector) where T : notnull
    {
        return selector(await runner.RunAll(args));
    }

    public struct Options
    {
        public int BatchSize;
        public int BatchTimespan;
        public int Delay;
        public int Timeout;

        public Options(int delay =0, int timeout = 0, int batchSize =0, int batchTimespan = 0)
        {
            BatchSize = batchSize;
            BatchTimespan = batchTimespan;
            Delay = delay;
            Timeout = timeout;
        }
    }
}
