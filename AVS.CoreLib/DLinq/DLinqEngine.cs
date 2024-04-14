using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.DLinq.Conditions;
using AVS.CoreLib.DLinq.Extensions;
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
    private const string WHERE = "WHERE ";
    private const string SKIP = "SKIP ";
    private const string TAKE = "TAKE ";
    private const string SORT = "ORDER BY ";
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
        {
            //select statement only
            if (q.IndexOf(',') == -1)
            {
                var spec = ValueExprSpec.Parse(q, type);
                return source.Select(spec, Mode);
            }

            var specs = ParseSelectExpr(q, type);
            return Select(source, specs);
        }

        //close WHERE close > 10000 SKIP 10 TAKE 2
        var parts = SplitQuery(q);

        var src = source;
        // Parse SELECT expr
        var specsDict = ParseSelectExpr(parts[0], type);

        // WHERE
        if (parts[1].Length > 0)
            src = Where(src, parts[1], type, specsDict);

        // ORDER BY
        if (parts[2].Length > 0)
            src = OrderBy(src, parts[2], type, specsDict);

        // SKIP
        if (parts[3].Length > 0)
            src = Skip(src, parts[3]);

        // TAKE
        if (parts[4].Length > 0)
            src = Take(src, parts[4]);

        return Select(src, specsDict);
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
        var startIndex = query.IndexOfAny(0, keywords, StringComparison.InvariantCultureIgnoreCase);

        // just a select statement
        if (startIndex == -1)
        {
            var res = new string [keywords.Length];
            res[0] = query.Trim();
            return res;
        }

        var selectExpr = startIndex > 0 ? query.Substring(0, startIndex) : string.Empty;
        var list = new List<string>(keywords.Length + 1) { selectExpr.Trim() };

        //query: close SKIP 10 TAKE 5 => [] { "close", "", "10", "5"}

        for (var i = 0; i < keywords.Length; i++)
        {
            var keyword = keywords[i];
            var index = query.IndexOf(keyword, startIndex, StringComparison.InvariantCultureIgnoreCase);

            if (index == -1)
            {
                list.Add(string.Empty);
                continue;
            }

            if (index == startIndex)
            {
                index += keyword.Length;
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
    
    private IEnumerable<T> Where<T>(IEnumerable<T> source, string whereExpr, Type targetType, Dictionary<string, ValueExprSpec> specs)
    {
        var condition = ConditionParser.Parse(whereExpr);
        var spec = condition.GetSpec(targetType, specs);
        var filtered = source.Where(spec, Mode);
        return filtered;
    }

    private IEnumerable<T> OrderBy<T>(IEnumerable<T> source, string orderByExpr, Type targetType, Dictionary<string, ValueExprSpec> specs)
    {
        // sortBy "close ASC"
        var length = orderByExpr.Length;
        var sortDirection = Sort.None;
        if (orderByExpr.EndsWith(" ASC", StringComparison.OrdinalIgnoreCase))
        {
            sortDirection = Sort.Asc;
            length -= 4;
        }
        else if (orderByExpr.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
        {
            sortDirection = Sort.Desc;
            length -= 5;
        }

        var orderByStr = orderByExpr.Substring(0, length);

        var parts = orderByStr.Split(',', StringSplitOptions.RemoveEmptyEntries);

        var valueExpr = parts[0].Trim();
        var spec = specs.ContainsKey(valueExpr) ? specs[valueExpr] :  ValueExprSpec.Parse(valueExpr, targetType);

        var ordered = source.OrderBy(spec, sortDirection, Mode);

        if (parts.Length > 1 && ordered is IOrderedEnumerable<T> orderedEnumerable)
        {
            for (var i = 1; i < parts.Length; i++)
            {
                valueExpr = parts[i].Trim();
                spec = specs.ContainsKey(valueExpr) ? specs[valueExpr] : ValueExprSpec.Parse(valueExpr, targetType);
                ordered = orderedEnumerable.ThenBy(spec, sortDirection, Mode);
            }
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

    private IEnumerable Select<T>(IEnumerable<T> source, Dictionary<string, ValueExprSpec> specs)
    {
        if (specs.Count == 0)
            return source;

        if (specs.Count == 1)
            return source.Select(specs.First().Value, Mode);

        var spec = new MultiPropExprSpec(specs);
        var result = source.Select(spec, Mode);
        return result;
    }

    private IEnumerable Select<T>(IEnumerable<T> source, string selectExpr, Type type)
    {
        if (selectExpr.IndexOf(',') == -1 && selectExpr.IndexOf(" as ", StringComparison.OrdinalIgnoreCase) == -1)
        {
            var spec = ValueExprSpec.Parse(selectExpr, type);
            return source.Select(spec, Mode);
        }

        var specs = ParseSelectExpr(selectExpr, type);
        return Select(source, specs);
    }

    //private ISpec ParseSelectExpr1(string selectExpr, Type argType)
    //{
    //    var parts = selectExpr.Split(',');

    //    if (parts.Length == 1)
    //        return ValueExprSpec.Parse(selectExpr.Trim(), argType);

    //    var spec = new MultiPropExprSpec(parts.Length) { Raw = selectExpr };

    //    foreach (var part in parts)
    //    {
    //        var valueSpec = ValueExprSpec.Parse(part.Trim(), argType);
    //        spec.AddSmart(valueSpec);
    //    }

    //    return spec;
    //}

    private Dictionary<string, ValueExprSpec> ParseSelectExpr(string selectExpr, Type argType)
    {
        var parts = selectExpr.Split(',');

        var specs = new Dictionary<string, ValueExprSpec>(parts.Length);

        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            var ind = part.IndexOf(" as ", StringComparison.OrdinalIgnoreCase);
            var exprStr = part;
            var alias = part;

            if (ind > -1)
            {
                exprStr = part.Substring(0, ind).Trim();
                alias = part.Substring(ind + 1).Trim();
            }

            if (specs.ContainsKey(alias))
            {
                alias = alias + "_" + i; //avoid collision
            }

            var spec = ValueExprSpec.Parse(exprStr, argType);
            specs.Add(alias, spec);
        }

        return specs;
    }
}