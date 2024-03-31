using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AVS.CoreLib.Extensions.Enums;
using AVS.CoreLib.Extensions.Linq;

namespace AVS.CoreLib.Extensions.Tasks;

public static class TaskExtensions
{
    public static async Task DoWhile(this Task task, Action otherAction)
    {
        otherAction.Invoke();
        await task;
    }

    public static async Task Then<T, TResult>(this Task<T> task, Action<T> then)
    {
        var result = await task;
        then(result);
    }

    public static async Task<TResult> Then<T, TResult>(this Task<T> task, Func<T, TResult> then)
    {
        var result = await task;
        return then(result);
    }

    public static async Task<IEnumerable<T>> OrderBy<T, Key>(this Task<List<T>> task, Func<T, Key> selector, Sort orderBy)
    {
        var result = await task;
        return result.OrderBy(selector, orderBy);
    }

    public static async Task<IEnumerable<T>> OrderBy<T, Key>(this Task<List<T>> task, Func<T, Key> selector)
    {
        var result = await task;
        return result.OrderBy(selector);
    }

    public static async Task<IEnumerable<T>> OrderByDescending<T, Key>(this Task<List<T>> task, Func<T, Key> selector)
    {
        var result = await task;
        return result.OrderByDescending(selector);
    }
}