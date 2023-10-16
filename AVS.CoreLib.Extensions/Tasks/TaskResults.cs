using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.Extensions.Linq;

namespace AVS.CoreLib.Extensions.Tasks;

public sealed class TaskResults<T, TResult> : IEnumerable<TResult>
{
    public Dictionary<T, TResult> Results { get; private set; }

    public TaskResults(int capacity = 0)
    {
        Results = new Dictionary<T, TResult>(capacity);
    }

    public void Add(T key, TResult value)
    {
        Results.Add(key,value);
    }

    public IEnumerable<(T, TResult)> GetFailed(Func<TResult, bool> isValid)
    {
        foreach (var kp in Results)
        {
            if (!isValid(kp.Value))
                yield return (kp.Key, kp.Value);
        }
    }

    public string? GetErrorsCombined(Func<TResult, string> selector, bool distinct = true)
    {
        var enumerable = Results.Values.Select(selector);
        if (distinct)
            enumerable = enumerable.Distinct();
        var errors = enumerable.ToArray();
        return errors.Any() ? string.Join(",", enumerable) : null;
    }

    public IEnumerator<TResult> GetEnumerator()
    {
        return Results.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Results.Values.GetEnumerator();
    }

    public static implicit operator Dictionary<T, TResult>(TaskResults<T, TResult> results) => results.Results;    
}

public static class TaskResultsExtensions
{
    public static List<TResult> ToList<T, TResult>(this TaskResults<T, TResult> tasks)
    {   
        return tasks.Results.Values.ToList();
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<TItem> PickUniqueItems<T,TResult, TItem, TKey>(this TaskResults<T,TResult> tasks,
        Func<TResult, IEnumerable<TItem>> selector, Func<TItem, TKey> keySelector, Func<TResult, bool>? isValid = null)
    {
        return 
            isValid == null 
            ? tasks.Results.Values.PickUniqueItems(selector, keySelector).Values.ToList()            
            : tasks.Results.Values.Where(isValid).PickUniqueItems(selector, keySelector).Values.ToList();
    }

    [DebuggerStepThrough]
    public static List<TItem> PickUniqueItems<T,TResult, TItem, TKey>(this TaskResults<T,TResult> tasks,  
        Func<TResult, IEnumerable<TItem>> selector,
        Func<TItem, TKey> keySelector, Enums.OrderBy orderDirection, Func<TResult, bool>? isValid = null)
    {
        var values = tasks.Results.Values.AsEnumerable();
        if (isValid != null)
        {
            values = values.Where(isValid);
        }
        var items = values.PickUniqueItems(selector, keySelector).Values;
        return items.OrderBy(keySelector, orderDirection).ToList();
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashSet<TItem> PickUniqueItems<T,TResult, TItem>(this TaskResults<T,TResult> tasks, Func<TResult, IEnumerable<TItem>> selector, Func<TResult, bool>? isValid = null)
    {
        return isValid == null ? tasks.Results.PickUniqueItems(selector) : tasks.Results.Values.Where(isValid).PickUniqueItems(selector);
    }
}