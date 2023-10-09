using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.Extensions.Stringify;

namespace AVS.CoreLib.Extensions.Tasks;

public sealed class TaskResults<T, TResult> where T : notnull
{
    public Dictionary<T, TResult> Results { get; }
    public Dictionary<T, TResult>? Failed { get; }
    public Dictionary<T, string>? Errors { get; }
    public bool HasErrors => Errors != null && Errors.Any();

    public TaskResults(Dictionary<T, TResult> results, Dictionary<T, TResult> failed, Dictionary<T, string> errors)
    {
        Results = results;

        if (failed.Any())
        {
            Failed = failed;
            Errors = errors;
        }
    }

    public string? GetCombinedErrors(bool distinct = true)
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

    public List<TItem> PickItems<TItem>(Func<TResult, IEnumerable<TItem>> selector)
    {
        return Results.PickItems(selector);
    }

    public List<TItem> PickItems<TItem>(Func<T, TResult, IEnumerable<TItem>> selector)
    {
        return Results.PickItems(selector);
    }

    public HashSet<TItem> PickUniqueItems<TItem>(Func<T, TResult, IEnumerable<TItem>> selector)
    {
        return Results.PickUniqueItems(selector);
    }

    public HashSet<TItem> PickUniqueItems<TItem>(Func<TResult, IEnumerable<TItem>> selector)
    {
        return Results.PickUniqueItems(selector);
    }

    public List<TItem> PickUniqueItems<TItem, TKey>(Func<TResult, IEnumerable<TItem>> selector, Func<TItem, TKey> keySelector)
    {
        return Results.PickUniqueItems(selector, keySelector).Values.ToList();
    }

    public List<TResult> ToList()
    {
        return Results.Values.ToList();
    }
}