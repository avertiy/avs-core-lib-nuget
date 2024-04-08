using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.DLinq.Conditions;
using AVS.CoreLib.DLinq.Extensions;
using AVS.CoreLib.DLinq.Specs;
using AVS.CoreLib.DLinq.Specs.CompoundBlocks;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Enums;

namespace AVS.CoreLib.DLinq;

/// <summary>
/// Process dynamic LINQ queries, supports DLinq query language
/// (pretty fluent and intuitive SELECT, TAKE, SKIP expressions, WHERE (not done yet))
/// some examples:
/// <code>
///     List{IBar} source = GetChartData();
///     engine.Process(source, "close"); => List{decimal}
///     engine.Process(source, "close,high"); => List{Dictionary{string,decimal}}
///     engine.Process(source, "close,high,bag[SMA21]", typeof(XBar)); => List{Dictionary{string,object}}
///     engine.Process(source, "close,high,bag[SMA21]", typeof(XBar));
///     engine.Process(source, "close,high,bag[SMA21].value,props[1] SKIP 20 TAKE 5", typeof(XBar));
/// </code>
/// </summary>
public class DLinqEngine
{
    private const string WHERE = "WHERE";
    private const string SKIP = "SKIP";
    private const string TAKE = "TAKE";
    private const string SORT = "SORT";
    private static bool IsAny(string expr) => expr is "*" or ".*";

    public SelectMode Mode { get; set; } = SelectMode.ToList;

    public IEnumerable Process<T>(IEnumerable<T> source, string query, Type? targetType)
    {
        if (IsAny(query))
            return source;

        var type = targetType ?? typeof(T);
        var ind = query.IndexOf(' ');

        var q = query.Trim();

        if (ind == -1)
            //select statement only
            return Select(source, q, type);


        //close WHERE close > 10000 SKIP 10 TAKE 2

        var parts = SplitQuery(q);

        var src = source;
        // WHERE
        if (parts[1].Length > 0)
            src = Where(src, parts[1], type);

        // SORT
        if (parts[2].Length > 0)
            src = SortBy(src, parts[2], type);

        // SKIP
        if (parts[3].Length > 0)
            src = Skip(src, parts[3]);

        // TAKE
        if (parts[4].Length > 0)
            src = Take(src, parts[4]);

        return Select(src, parts[0], type);
    }

    /// <summary>
    /// [0] - select expression
    /// [1] - where expression
    /// [2] - skip expression
    /// [3] - take expression
    /// </summary>
    private string[] SplitQuery(string query)
    {
        var keywords = new[] { WHERE, SORT, SKIP, TAKE };
        var startIndex = query.IndexOfAny(0, keywords);

        // just a select statement
        if (startIndex == -1)
        {
            var res = new string [keywords.Length];
            res[0] = query;
            return res;
        }

        var selectExpr = startIndex > 0 ? query.Substring(0, startIndex) : string.Empty;
        var list = new List<string>(keywords.Length + 1) { selectExpr.TrimEnd() };

        //query: close SKIP 10 TAKE 5 => [] { "close", "", "10", "5"}

        for (var i = 0; i < keywords.Length; i++)
        {
            var keyword = keywords[i];
            var index = query.IndexOf(keyword, startIndex, StringComparison.Ordinal);

            if (index == -1)
            {
                list.Add(string.Empty);
                continue;
            }

            if (index == startIndex)
            {
                index += keyword.Length+1;
                var nextInd = i + 1 < keywords.Length ? query.IndexOfAny(index, keywords.Skip(1)) : -1;


                var str = nextInd == -1 ? query.Substring(index) : query.Substring(index, nextInd-index);
                list.Add(str.Trim());

                startIndex = nextInd > -1 ? nextInd : index;
                continue;
            }

            throw new Exception("something wrong in this logic");
        }

        return list.ToArray();
    }
    
    private IEnumerable<T> Where<T>(IEnumerable<T> source, string whereExpr, Type targetType)
    {
        var condition = ConditionParser.Parse(whereExpr);
        var spec = condition.GetSpec(targetType);
        var filtered = source.Where(spec, Mode);
        return filtered;
    }

    private IEnumerable<T> SortBy<T>(IEnumerable<T> source, string sortBy, Type targetType)
    {
        // sortBy "close ASC"
        var length = sortBy.Length;
        var sortDirection = Sort.None;
        if (sortBy.EndsWith(" ASC", StringComparison.OrdinalIgnoreCase))
        {
            sortDirection = Sort.Asc;
            length -= 4;
        }
        else if (sortBy.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
        {
            sortDirection = Sort.Desc;
            length -= 5;
        }

        var orderByStr = sortBy.Substring(0, length);

        var parts = orderByStr.Split(',', StringSplitOptions.RemoveEmptyEntries);

        var spec = ValueExprSpec.Parse(parts[0], targetType);

        var ordered = source.OrderBy(spec, sortDirection, Mode);

        if (parts.Length > 1 && ordered is IOrderedEnumerable<T> orderedEnumerable)
        {
            spec = ValueExprSpec.Parse(parts[1], targetType);
            ordered = orderedEnumerable.ThenBy(spec, sortDirection, Mode);
        }

        return ordered;
    }

    private IEnumerable<T> Take<T>(IEnumerable<T> source, string takeExpr)
    {
        return int.TryParse(takeExpr, out var take) ? source.Take(take) : source;
    }

    private IEnumerable<T> Skip<T>(IEnumerable<T> source, string skipExpr)
    {
        return int.TryParse(skipExpr, out var skip) ? source.Skip(skip) : source;
    }

    private IEnumerable Select<T>(IEnumerable<T> source, string selectExpr, Type argType)
    {
        var spec = ParseSelectExpr1(selectExpr, argType);
        var result = source.Select(spec, Mode);
        return result;
    }

    private ISpec ParseSelectExpr1(string selectExpr, Type argType)
    {
        var parts = selectExpr.Split(',');

        if (parts.Length == 1)
            return ValueExprSpec.Parse(selectExpr, argType);

        var spec = new MultiPropExprSpec(parts.Length) { Raw = selectExpr };

        foreach (var part in parts)
        {
            var valueSpec = ValueExprSpec.Parse(part, argType);
            spec.AddSmart(valueSpec);
        }

        return spec;
    }
}