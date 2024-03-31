using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.DLinq.LambdaSpec;
using AVS.CoreLib.DLinq0.LambdaSpec0;
using AVS.CoreLib.Extensions;

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
    private static bool IsAny(string expr) => expr is "*" or ".*";

    public SpecMode Mode { get; set; } = SpecMode.ToList;

    public IEnumerable Process<T>(IEnumerable<T> source, string query, Type? targetType)
    {
        if (IsAny(query))
            return source;

        var argType = targetType ?? typeof(T);
        var ind = query.IndexOf(' ');

        var q = query.Trim();

        if (ind == -1)
            //select statement only
            return Select(source, q, argType);


        //close WHERE close > 10000 SKIP 10 TAKE 2

        var parts = SplitQuery(q);

        var src = source;
        // WHERE
        if (parts[1].Length > 0)
            src = Where(source, parts[1], argType);

        // SKIP
        if (parts[2].Length > 0)
            src = Skip(src, parts[2]);

        // TAKE
        if (parts[3].Length > 0)
            src = Take(src, parts[3]);

        return Select(src, parts[0], argType);
    }

    /// <summary>
    /// [0] - select expression
    /// [1] - where expression
    /// [2] - skip expression
    /// [3] - take expression
    /// </summary>
    private string[] SplitQuery(string query)
    {
        var keywords = new[] { WHERE, SKIP, TAKE };
        var startIndex = query.IndexOfAny(0, keywords);

        // just a select statement
        if (startIndex == -1)
            return new[] { query };

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

    private IEnumerable<T> Take<T>(IEnumerable<T> source, string takeExpr)
    {
        return int.TryParse(takeExpr, out var take) ? source.Take(take) : source;
    }

    private IEnumerable<T> Skip<T>(IEnumerable<T> source, string skipExpr)
    {
        return int.TryParse(skipExpr, out var skip) ? source.Skip(skip) : source;
    }

    private IEnumerable<T> Where<T>(IEnumerable<T> source, string whereExpr, Type targetType)
    {
        return source;
    }

    private IEnumerable Select<T>(IEnumerable<T> source, string selectExpr, Type argType)
    {
        var spec = ParseSelectExpr(selectExpr);
        spec.ArgType = argType;
        var result = LambdaBag.Lambdas.Execute(spec, source);
        return result;
    }

    private Spec ParseSelectExpr(string selectExpr)
    {
        var parts = selectExpr.Split(',');

        if (parts.Length == 1)
            ParseValueExpr(selectExpr);

        var spec = new MultiPropExprSpec(parts.Length) { Mode = Mode, Raw = selectExpr };

        foreach (var part in parts)
        {
            var specItem = ParseValueExpr(part);
            var key = specItem.ToString(SpecView.Key);
            spec.AddSmart(key, specItem);
        }

        return spec;
    }

    private ValueExprSpec ParseValueExpr(string selectExpr)
    {
        var expr = selectExpr.TrimStart('.');
        var startInd = 0;

        if (selectExpr.StartsWith("x."))
            startInd = 2;

        var spec = new ValueExprSpec() { Mode = Mode, Raw = expr };
        var ind = -1;

        for (var i = startInd; i < expr.Length; i++)
            switch (expr[i])
            {
                case '.':
                    {
                        //prop.inner.value or prop[0].inner
                        if (startInd < i && ind == -1)
                            spec.AddProp(expr.Substring(startInd, i - startInd));
                        startInd = i + 1;
                        break;
                    }

                case '[' when ind < 0:
                    {
                        if (startInd < i)
                            spec.AddProp(expr.Substring(startInd, i - startInd));
                        startInd = i + 1;
                        ind = i;
                        break;
                    }
                case ']' when ind > -1:
                    {
                        var key = expr.Substring(ind + 1, i - ind - 1);
                        spec.AddIndex(key);
                        ind = -1;
                        startInd = i + 1;
                        break;
                    }
            }

        if (startInd < expr.Length)
            spec.AddProp(expr.Substring(startInd, expr.Length - startInd));

        return spec;
    }
}