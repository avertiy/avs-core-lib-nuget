using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Extensions.Tasks;

namespace AVS.CoreLib.Tasks;

public class TaskRunner<TResult>
{
    private readonly Delegate _job;
    private readonly Func<TResult, bool>? _isSuccess;
    public TaskRunnerOptions Options { get; set; }

    private readonly bool _invokeWithCancellationToken;

    [DebuggerStepThrough]
    internal TaskRunner(Delegate job,
        TaskRunnerOptions options,
        Func<TResult, bool>? isSuccess,
        bool invokeWithCancellationToken)
    {
        if (options.Strategy == TaskRunnerStrategy.ExecuteOneThenAll && isSuccess == null)
            throw new ArgumentException($"{nameof(TaskRunnerStrategy.ExecuteOneThenAll)} strategy requires {nameof(isSuccess)} delegate");

        _job = job;
        _isSuccess = isSuccess;
        Options = options;
        _invokeWithCancellationToken = invokeWithCancellationToken;
    }

    public Task<TResult> InvokeFunc<T>(T arg, CancellationToken token = default)
    {
        if (_invokeWithCancellationToken)
        {
            var func = (Func<T, CancellationToken, Task<TResult>>)_job;
            return func(arg, token);
        }
        else
        {
            var func = (Func<T, Task<TResult>>)_job;
            return func(arg);
        }
    }

    public async Task<TaskResults<T, TResult>> ExecuteAll<T>(IList<T> args, CancellationToken ct) where T : notnull
    {
        try
        {
            TaskResults<T, TResult> results;

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            if (Options.Timeout > 0)
                cts.CancelAfter(Options.Timeout);

            switch (Options.Strategy)
            {
                case TaskRunnerStrategy.ExecuteOneThenAll:
                    results = await ExecuteOneThenAll(args, cts.Token);
                    break;
                default:
                {
                    results = Options.BatchSize > 0
                        ? await ExecuteInBatchMode(args, cts.Token)
                        : await ExecuteInDefaultMode(args, cts.Token);
                    break;
                }
            }

            return results;
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            throw new TimeoutException("The operation timed out.");
        }
    }

    public async Task<TaskResults<T, TResult>> ExecuteOne<T>(T arg, CancellationToken ct) where T : notnull
    {
        var res = await InvokeFunc(arg, ct);
        return new TaskResults<T, TResult> { { arg, res } };
    }


    internal async Task<TaskResults<T, TResult>> ExecuteOneThenAll<T>(IList<T> args, CancellationToken ct) where T : notnull
    {
        var result = await InvokeFunc(args[0], ct);

        if (!IsSuccess(result))
            return new TaskResults<T, TResult> { { args[0], result } };

        var results = Options.BatchSize > 0
            ? await ExecuteInBatchMode(args.Skip(1).ToArray(), ct)
            : await ExecuteInDefaultMode(args.Skip(1).ToArray(), ct);

        results.Add(args[0], result);
        return results;
    }

    private async Task<TaskResults<T, TResult>> ExecuteInDefaultMode<T>(ICollection<T> args, CancellationToken ct) where T : notnull
    {
        var results = new TaskResults<T, TResult>(args.Count + 1);

        var tasks = Start(args, ct);
        await Task.WhenAll(tasks.Values);
        
        foreach (var kp in tasks)
        {
            var res = kp.Value.Result;
            results.Add(kp.Key, res);
        }

        return results;
    }

    private async Task<TaskResults<T, TResult>> ExecuteInBatchMode<T>(ICollection<T> args, CancellationToken ct) where T : notnull
    {
        var batchSize = Options.BatchSize;
        var results = new TaskResults<T, TResult>(args.Count+1);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        //var fn = GetFunc<T>();
        var queue = new Queue<(T key, Task<TResult> result)>(batchSize + 1);
        var elapsed = 0;

        foreach (var arg in args)
        {
            if (ct.IsCancellationRequested)
                break;

            queue.Enqueue((arg, InvokeFunc(arg, ct)));
            Sleep(Options.Delay);

            if (queue.Count == batchSize)
            {
                await processQueue();
                var delay = Options.BatchTimespan - (int)stopwatch.ElapsedMilliseconds - elapsed;
                Sleep(delay);
                elapsed = (int)stopwatch.ElapsedMilliseconds;
            }
        }

        await processQueue();

        return results;

        async Task processQueue()
        {
            while (queue.Count > 0)
            {
                if (ct.IsCancellationRequested)
                    break;

                var (key, task) = queue.Dequeue();
                await task;
                results.Add(key, task.Result);
            }
        }
    }

    private Dictionary<T, Task<TResult>> Start<T>(IEnumerable<T> args, CancellationToken ct) where T : notnull
    {
        var tasks = new Dictionary<T, Task<TResult>>();

        foreach (var arg in args)
        {
            if (ct.IsCancellationRequested)
                break;

            var task = InvokeFunc(arg, ct);
            tasks.Add(arg, task);
            Sleep(Options.Delay);
        }

        return tasks;
    }

    internal bool IsSuccess(TResult result) => _isSuccess?.Invoke(result) == true;

    private static void Sleep(int delay)
    {
        if (delay > 0)
            Thread.Sleep(delay);
    }
}

public static class TaskRunner
{
    [DebuggerStepThrough]
    public static TaskRunner<TResult> Create<T, TResult>(Func<T, CancellationToken, Task<TResult>> job, TaskRunnerOptions options, Func<TResult, bool>? isSuccess = null)
    {
        return new TaskRunner<TResult>(job, options, isSuccess, true);
    }

    [DebuggerStepThrough]
    public static TaskRunner<TResult> Create<T, TResult>(Func<T, Task<TResult>> job, TaskRunnerOptions options, Func<TResult, bool>? isSuccess = null)
    {
        return new TaskRunner<TResult>(job, options, isSuccess, false);
    }

    [DebuggerStepThrough]
    public static Task<TaskResults<T, TResult>> RunAll<T, TResult>(this TaskRunner<TResult> runner, IList<T> args, CancellationToken ct = default) where T : notnull
    {
        return args.Count switch
        {
            0 => Task.FromResult(new TaskResults<T, TResult>()),
            1 => runner.ExecuteOne(args[0], ct),
            _ => runner.ExecuteAll(args, ct),
        };
    }

    [DebuggerStepThrough]
    public static async Task<TaskResults<T, TResult>> RunAll<T, TResult>(this TaskRunner<TResult> runner, IEnumerable<T> args, CancellationToken ct = default) where T : notnull
    {
        if (runner.Options.Strategy != TaskRunnerStrategy.ExecuteOneThenAll)
        {
            return await runner.ExecuteAll(args.ToArray(), ct);
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        if (runner.Options.Timeout > 0)
            cts.CancelAfter(runner.Options.Timeout);

        var argsList = new List<T>();
        var results = new TaskResults<T, TResult>();

        foreach (var arg in args)
        {
            if (results.Count > 0)
            {
                argsList.Add(arg);
                continue;
            }

            var res = await runner.InvokeFunc(arg, cts.Token);
            results.Add(arg, res);

            if (!runner.IsSuccess(res))
                break;
        }

        if (argsList.Count > 0)
        {
            results.Add(await runner.ExecuteAll(argsList, cts.Token));
        }

        return results;
    }
}


/*
public class TaskRunner<TResult>
{
    private readonly Delegate _job;
    
    [DebuggerStepThrough]
    internal TaskRunner(Delegate job)
    {
        this._job = job;
    }

    #region Props (TaskRunner Options)
    /// <summary>
    /// delay in milliseconds between tasks
    /// </summary>
    public int Delay { get; set; }
    /// <summary>
    /// timeout in milliseconds
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

    public TaskRunnerStrategy Strategy { get; set; } 
    #endregion


    public Func<T, Task<TResult>> GetFunc<T>()
    {
        return (Func<T, Task<TResult>>)_job;
    }

    public async Task<TaskResults<T, TResult>> ExecuteOne<T>(T arg) where T : notnull
    {
        var fn = GetFunc<T>();
        var res = await fn(arg);

        var results = new TaskResults<T, TResult>
        {
            { arg, res }
        };
        return results;
    }

    public async Task<TaskResults<T, TResult>> RunAll<T>(IEnumerable<T> args, CancellationToken ct = default) where T : notnull
    {
        if (BatchSize > 0)
        {
            return await ExecuteInBatchMode(args, ct);
        }

        var tasks = StartAll(args);
        await Task.WhenAll(tasks.Values);

        var results = new TaskResults<T, TResult>(tasks.Count);
        foreach (var kp in tasks)
        {
            var res = kp.Value.Result;
            results.Add(kp.Key, res);
        }

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

    private async Task<TaskResults<T, TResult>> ExecuteInBatchMode<T>(IEnumerable<T> args, CancellationToken ct) where T : notnull
    {
        var fn = (Func<T, Task<TResult>>)_job;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var queue = new Queue<(T key, Task<TResult> result)>(BatchSize + 1);
        var results = new TaskResults<T, TResult>(BatchSize * 2);

        var processQueue = async () =>
        {
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
                elapsed = (int)stopwatch.ElapsedMilliseconds;
            }
        }

        await processQueue();

        return results;
    }

    

    private static void Sleep(int delay)
    {
        if (delay > 0)
            Thread.Sleep(delay);
    }

    //[DebuggerStepThrough]
    //public TaskRunner<TResult> WithDelay(int delayInMilliseconds)
    //{
    //    Delay = delayInMilliseconds;
    //    return this;
    //}

    //[DebuggerStepThrough]
    //public TaskRunner<TResult> WithTimeout(int timeout)
    //{
    //    Timeout = timeout;
    //    return this;
    //}

    //[DebuggerStepThrough]
    //public TaskRunner<TResult> UseBatchMode(int batchSize, int batchDelay)
    //{
    //    BatchSize = batchSize;
    //    BatchTimespan = batchDelay;
    //    return this;
    //}
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
    public static TaskRunner<TResult> Create<T, TResult>(Func<T, Task<TResult>> job, TaskRunnerOptions taskRunnerOptions)
    {
        return new TaskRunner<TResult>(job)
        {
            Delay = taskRunnerOptions.Delay,
            Timeout = taskRunnerOptions.Timeout,
            BatchSize = taskRunnerOptions.BatchSize,
            BatchTimespan = taskRunnerOptions.BatchTimespan
        };
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
}

*/