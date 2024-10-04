using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Extensions.Tasks;

namespace AVS.CoreLib.Tasks;

public static class ParallelExtensions
{
    /// <summary>
    /// Run job for each item in parallel mode <see cref="TaskRunner"/>
    /// returns <see cref="TaskResults{T,TResult}"/>
    /// </summary>    
    [DebuggerStepThrough]
    public static Task<TaskResults<T, TResult>> RunInParallel<T, TResult>(this IEnumerable<T> args, Func<T, CancellationToken, Task<TResult>> func, int delay = 0, CancellationToken ct = default) where T : notnull
    {
        return TaskRunner.Create(func, new TaskRunnerOptions(delay)).RunAll(args, ct);
    }

    [DebuggerStepThrough]
    public static Task<TaskResults<T, TResult>> RunInParallel<T, TResult>(this IEnumerable<T> args, Func<T, CancellationToken, Task<TResult>> func, TaskRunnerOptions options, CancellationToken ct = default) where T : notnull
    {
        return TaskRunner.Create(func, options).RunAll(args, ct);
    }

    [DebuggerStepThrough]
    public static Task<TaskResults<T, TResult>> RunInParallel<T, TResult>(this IList<T> args, Func<T, CancellationToken, Task<TResult>> func, int delay = 0, CancellationToken ct = default) where T : notnull
    {
        return TaskRunner.Create(func, new TaskRunnerOptions(delay)).RunAll(args, ct);
    }

    [DebuggerStepThrough]
    public static Task<TaskResults<T, TResult>> RunInParallel<T, TResult>(this IList<T> args, Func<T, Task<TResult>> func, int delay = 0, CancellationToken ct = default) where T : notnull
    {
        return TaskRunner.Create(func, new TaskRunnerOptions(delay)).RunAll(args, ct);
    }

    [DebuggerStepThrough]
    public static Task<TaskResults<T, TResult>> RunInParallel<T, TResult>(this IList<T> args, Func<T, CancellationToken, Task<TResult>> func, TaskRunnerOptions options, CancellationToken ct = default) where T : notnull
    {
        return TaskRunner.Create(func, options).RunAll(args, ct);
    }

    /*
    [DebuggerStepThrough]
    public static Task<TaskResults<T, TResult>> RunInParallel<T, TResult>(this T[] args, Func<T, CancellationToken, Task<TResult>> job, int delay = 0, CancellationToken ct = default) where T : notnull
    {
        return TaskRunner.Create(job, new TaskRunnerOptions(delay)).RunAll(args, ct);
    }

    [DebuggerStepThrough]
    public static Task<TaskResults<T, TResult>> RunInParallel<T, TResult>(this T[] args, Func<T, CancellationToken, Task<TResult>> job, TaskRunnerOptions options, CancellationToken ct = default) where T : notnull
    {
        return TaskRunner.Create(job, options).RunAll(args, ct);
    }*/

    ///// <summary>
    ///// for each item creates job (task), executes them in in parallel, then returns results <see cref="List{TResult}"/>
    ///// </summary>
    //[DebuggerStepThrough]
    //[Obsolete("Are there any usages?")]
    //public static Task<List<TResult>> ParallelFetch<T, TResult>(this IEnumerable<T> enumerable, Func<T, Task<TResult>> job, int delay = 0) where T : notnull
    //{
    //    return TaskRunner.Create(job, delay).RunAll(enumerable, x => x.ToList());
    //}


    //[DebuggerStepThrough]
    //[Obsolete("Are there any usages?")]
    //public static Task<List<TItem>> ParallelFetchUniqueItems<T, TResult, TItem, TItemKey>(this IEnumerable<T> enumerable,
    //    Func<T, Task<TResult>> job,
    //    Func<TResult, IEnumerable<TItem>> selector, Func<TItem, TItemKey> itemKeySelector, int delay = 0) where T : notnull
    //{
    //    return TaskRunner.Create(job, delay).RunAll(enumerable, x => x.PickUniqueItems(selector, itemKeySelector));
    //}
}