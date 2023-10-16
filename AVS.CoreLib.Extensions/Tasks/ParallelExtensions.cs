using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AVS.CoreLib.Extensions.Tasks;

public static class ParallelExtensions
{
    /// <summary>
    /// Run job for each item in parallel mode <see cref="TaskRunner"/>
    /// returns <see cref="TaskResults{T,TResult}"/>
    /// </summary>    
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TaskResults<T, TResult>> RunInParallel<T, TResult>(this IEnumerable<T> args, Func<T, Task<TResult>> job, int delay = 0)
    {
        return TaskRunner.Create(job, delay).RunAll(args);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TaskResults<T, TResult>> RunInParallel<T, TResult>(this T[] args, Func<T, Task<TResult>> job, int delay = 0)
    {
        return TaskRunner.Create(job, delay).RunAll(args);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TaskResults<T, TResult>> RunInParallel<T, TResult>(this IList<T> args, Func<T, Task<TResult>> job, int delay = 0)
    {
        return TaskRunner.Create(job, delay).RunAll(args);
    }

    /// <summary>
    /// for each item creates job (task), executes them in in parallel, then returns results <see cref="List{TResult}"/>
    /// </summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<List<TResult>> ParallelFetch<T, TResult>(this IEnumerable<T> enumerable, Func<T, Task<TResult>> job, int delay = 0)
    {
        return TaskRunner.Create(job, delay).RunAll(enumerable, x => x.ToList());
    }

    
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<List<TItem>> ParallelFetchUniqueItems<T, TResult, TItem, TItemKey>(this IEnumerable<T> enumerable,
        Func<T, Task<TResult>> job, 
        Func<TResult, IEnumerable<TItem>> selector, Func<TItem, TItemKey> itemKeySelector, int delay = 0)
    {
        return TaskRunner.Create(job, delay).RunAll(enumerable, x => x.PickUniqueItems(selector, itemKeySelector));
    }
}