using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.Extensions.Linq;

namespace AVS.CoreLib.Extensions.Tasks;

public sealed class TaskResults<T, TResult> : IEnumerable<KeyValuePair<T,TResult>>
{
    internal Dictionary<T, TResult> Items { get; private set; }
    public int Count => Items.Count;
    /// <summary>
    /// Calculates total count of items of the TResult 
    /// i.e. if TResult is countable in either way 
    /// (i) has Count property
    /// (ii) IEnumerable
    /// (iii) has countable Data property e.g. Response.Data.Count or Response.Data.Items.Count
    /// </summary>
    public int TotalCount => Items.Values.Sum(x => x.GetCount() ?? 0);

    public IReadOnlyCollection<T> Keys => Items.Keys;
    public IReadOnlyCollection<TResult> Values => Items.Values;

    public TaskResults(int capacity = 0)
    {
        Items = new Dictionary<T, TResult>(capacity);        
    }

    public void Add(T key, TResult value)
    {
        Items.Add(key,value);
    }

    public TOutput? Peek<TOutput>(Func<TResult, TOutput> selector)
    {
        return Items.Values.Select(selector).FirstOrDefault();
    }

    public TResult GetValue(Func<TResult, bool>? predicate = null)
    {
        return predicate == null ? Items.Values.FirstOrDefault() : Items.Values.FirstOrDefault(predicate);
    }

    public IEnumerable<TResult> GetValues(Func<TResult, bool> predicate)
    {
        return Items.Values.Where(predicate);
    }

    public IEnumerable<TResult> GetValues(Func<KeyValuePair<T,TResult>, bool> predicate)
    {
        return Items.Where(predicate).Select(x => x.Value);
    }

    public TResult this[T key] => Items[key];

    public IEnumerator<KeyValuePair<T, TResult>> GetEnumerator()
    {
        return Items.GetEnumerator();        
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    public override string ToString()
    {
        var sb = new StringBuilder($"TaskResults<{typeof(T).Name},{typeof(TResult).Name}>");

        sb.Append($"(#{Count}");

        if(Count > 0)
        {
            if (Items.Any(kp => kp.Value is IResponse))
            {
                var success = Items.Values.Count(x => ((IResponse)x).Success);
                var failed = Items.Values.Count(x => ((IResponse)x).Success == false);

                if(success > 0)
                    sb.Append($" {success} - OK;");

                if (failed > 0)
                    sb.Append($" {failed} - FAIL;");
            }
            var totalCount = TotalCount;

            if(totalCount > 0)
                sb.Append($" items #{totalCount};");
        }

        sb.Append(')');
        return sb.ToString();
    }
}

public static class TaskResultsExtensions
{
    public static string? GetErrors<T,TResult>(this TaskResults<T, TResult> results, Func<TResult, string?> selector, Func<T, string>? keySelector = null)
    {
        if (results.Count == 0)
            return null;

        var sb = new StringBuilder();

        foreach (var kp in results.Items)
        {
            var error = selector(kp.Value);
            if (error == null)
                continue;

            if(keySelector == null)
                sb.Append($"{error}; ");
            else 
                sb.Append($"{keySelector(kp.Key)}: {error}; ");
        }

        if (sb.Length == 0)
            return null;

        sb.Length -= 2;
        return sb.ToString();
    }

    public static List<TResult> ToList<T, TResult>(this TaskResults<T, TResult> results)
    {   
        return results.Items.Values.ToList();
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<TItem> PickUniqueItems<T,TResult, TItem, TKey>(this TaskResults<T,TResult> tasks,
        Func<TResult, IEnumerable<TItem>> selector, Func<TItem, TKey> keySelector, Func<TResult, bool>? isValid = null)
    {
        return 
            isValid == null 
            ? tasks.Items.Values.PickUniqueItems(selector, keySelector).Values.ToList()            
            : tasks.Items.Values.Where(isValid).PickUniqueItems(selector, keySelector).Values.ToList();
    }

    [DebuggerStepThrough]
    public static List<TItem> PickUniqueItems<T,TResult, TItem, TKey>(this TaskResults<T,TResult> tasks,  
        Func<TResult, IEnumerable<TItem>> selector,
        Func<TItem, TKey> keySelector, Enums.Sort orderDirection, Func<TResult, bool>? isValid = null)
    {
        var values = tasks.Items.Values.AsEnumerable();
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
        return isValid == null ? tasks.Items.PickUniqueItems(selector) : tasks.Items.Values.Where(isValid).PickUniqueItems(selector);
    }
}