using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.DLinq.LambdaSpec;
using AVS.CoreLib.DLinq0.LambdaSpec0;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.DLinq;

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

        if (ind == -1)
            //select statement only
            return Select(source, query, argType);


        //close,time WHERE close > 10000 SKIP 10 TAKE 2

        var parts = SplitQuery(query);

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

        return Select(src, query, argType);
    }

    /// <summary>
    /// 0 - select expression
    /// 1 - where expression
    /// 2 - skip expression
    /// 3 - take expression
    /// </summary>
    private string[] SplitQuery(string query)
    {
        var keywords = new[] { WHERE, SKIP, TAKE };
        var startIndex = query.IndexOfAny(keywords);

        // just a select statement
        if (startIndex == -1)
            return new[] { query };

        var selectExpr = startIndex > 0 ? query.Substring(0, startIndex) : string.Empty;

        var list = new List<string>(keywords.Length + 1) { selectExpr };

        foreach (var keyword in keywords)
        {
            var index = query.IndexOf(keyword, startIndex, StringComparison.Ordinal);
            var str = string.Empty;
            if (index >= 0)
            {
                str = query.Substring(startIndex, index);
                startIndex = index + keyword.Length;
            }

            list.Add(str);
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

        var spec = new MultiPropExprSpec(parts.Length);

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

        var spec = new ValueExprSpec() { Raw = expr, Mode = Mode };
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