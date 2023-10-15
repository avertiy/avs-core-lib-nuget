using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.Extensions.Linq;
using AVS.CoreLib.Extensions.Stringify;

namespace AVS.CoreLib.Extensions.Tasks;

public sealed class TaskResults<T, TResult> : IEnumerable<TResult>
{
    public Dictionary<T, TResult> Results { get; private set; }
    public Dictionary<T, TResult>? Failed { get; private set; }
    public Dictionary<T, string>? Errors { get; private set; }

    public TaskResults(int capacity = 0)
    {
        Results = new Dictionary<T, TResult>(capacity);
    }

    public void Add(T key, TResult value, string? err = null)
    {
        if (err != null)
        {
            Failed ??= new Dictionary<T, TResult>();
            Errors ??= new Dictionary<T, string>();
            Errors[key] = err;
            Failed[key] = value;
        }
        else
        {
            Results[key] = value;
        }
    }

    public bool HasErrors => Errors != null && Errors.Any();

    public Dictionary<T, TResult> GetAllResults()
    {
        var dict = new Dictionary<T, TResult>(Results);
        if (Failed != null)
        {
            foreach (var kp in Failed)
            {
                dict.Add(kp.Key, kp.Value);
            }
        }
        return dict;
    }

    public string? GetErrorsCombined(bool distinct = true)
    {
        if (Errors == null || Errors.Count == 0)
            return null;

        if (!distinct)
            return Errors.Stringify();

        var distinctErrors = Errors.Values.Distinct().ToArray();

        return distinctErrors.Length == 1
            ? $"{string.Join(",", Errors.Keys)}: {distinctErrors[0]}"
            : Errors.Stringify();
    }

    public string[] GetErrors(bool distinct = true)
    {
        if (Errors == null || Errors.Count == 0)
            return Array.Empty<string>();

        if (!distinct)
            return Errors.Values.ToArray();

        var errors = Errors.Values.Distinct().ToArray();
        return errors;
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

    public static implicit operator string[](TaskResults<T, TResult> results) => results.Errors?.Values.ToArray() ?? Array.Empty<string>();
}

public static class TaskResultsExtensions
{
    public static List<TResult> ToList<T, TResult>(this TaskResults<T, TResult> tasks)
    {   
        return tasks.Results.Values.ToList();
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<TItem> PickItems<T, TResult, TItem>(this TaskResults<T, TResult> tasks, Func<TResult, IEnumerable<TItem>> selector)
    {
        return tasks.Results.PickItems(selector);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<TItem> PickItems<T,TResult, TItem>(this TaskResults<T,TResult> tasks, Func<T, TResult, IEnumerable<TItem>> selector)
    {
        return tasks.Results.PickItems((key, res) => selector(key, res));
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<TItem> PickUniqueItems<T,TResult, TItem, TKey>(this TaskResults<T,TResult> tasks, 
        Func<TResult, IEnumerable<TItem>> selector, Func<TItem, TKey> keySelector)
    {
        return tasks.Results.PickUniqueItems(selector, keySelector).Values.ToList();
    }

    [DebuggerStepThrough]
    public static List<TItem> PickUniqueItems<T,TResult, TItem, TKey>(this TaskResults<T,TResult> tasks, Func<TResult, IEnumerable<TItem>> selector,
        Func<TItem, TKey> keySelector, Enums.OrderBy orderDirection)
    {
        var values = tasks.Results.PickUniqueItems(selector, keySelector).Values;
        return values.OrderBy(keySelector, orderDirection).ToList();
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashSet<TItem> PickUniqueItems<T,TResult, TItem>(this TaskResults<T,TResult> tasks, Func<TResult, IEnumerable<TItem>> selector)
    {
        return tasks.Results.PickUniqueItems(selector);
    }
}