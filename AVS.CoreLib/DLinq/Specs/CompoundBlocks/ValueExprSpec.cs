using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Specs.BasicBlocks;
using AVS.CoreLib.Expressions;
using AVS.CoreLib.Extensions.Enums;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Specs.CompoundBlocks;

/// <summary>
/// Represent a lambda specification single field (value) selector
/// e.g. x => x.close or more complex: x => x.prop[0]['key1'].value 
/// Contain one or more building blocks: <see cref="PropSpec"/>, <see cref="IndexSpec"/>, <see cref="KeySpec"/>
/// </summary>
public class ValueExprSpec : SpecBase
{
    public AggregateFn Fn { get; set; }
    public string? Alias { get; set; }
    public List<PropSpec> Parts { get; private set; } = new();
    public required Type ArgType { get; set; }

    public Type? ReturnType { get; set; }

    public bool IsEmpty => Parts.Count == 0;

    public void AddProp(string prop)
    {
        Parts.Add(new PropSpec() { Name = prop, Raw = Raw });
    }

    public void AddIndex(string input)
    {
        if (int.TryParse(input, out var index))
            Parts.Add(new IndexSpec(index) { Raw = Raw });
        else
            Parts.Add(new KeySpec(input.Trim('"', '\'')) { Raw = Raw });
    }

    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        var expr = expression.Type == ArgType ? expression : Expression.Convert(expression, ArgType);

        for (var i = 0; i < Parts.Count; i++)
            expr = Parts[i].BuildExpr(expr, ctx);

        if(ReturnType != null)
            expr = Expression.Convert(expr, ReturnType);

        if (ctx.Mode.HasFlag(SelectMode.Safe))
            expr = Expr.WrapInTryCatch(expr);

        return expr;
    }

    public override string ToString(string arg, SpecView view = SpecView.Default)
    {
        //var str = view == SpecView.Plain ? arg : $"(({ArgType.GetReadableName()}){arg})";
        var str = $"(({ArgType.GetReadableName()}){arg})";

        for (var i = 0; i < Parts.Count; i++)
            str = Parts[i].ToString(str, view);

        return str;
    }

    public static ValueExprSpec Parse(string expr, Type type)
    {
        return ValueExprSpecHelper.Parse(expr, type);
    }
}

internal static class ValueExprSpecHelper
{
    private const string MAX = "MAX(";
    private const string MIN = "MIN(";
    private const string AVG = "AVG(";
    private const string SUM = "SUM(";
    private const string AS = " AS ";
    public static ValueExprSpec Parse(string valueExpr, Type type)
    {
        var expr = valueExpr.TrimStart('.');
        var startInd = 0;
        //if (valueExpr.StartsWith("x."))
        //    startInd = 2;
        var endInd = expr.Length;

        var fn = GetAggregateFn(expr);

        var spec = new ValueExprSpec() { Fn = fn, ArgType = type, Raw = expr };

        var aliasInd = expr.IndexOf(AS, startInd, StringComparison.InvariantCultureIgnoreCase);

        if (aliasInd > -1)
        {
            spec.Alias = expr.Substring(aliasInd + AS.Length).Trim();
            endInd = aliasInd;
        }

        if (fn > 0)
        {
            startInd += MAX.Length;
            endInd--;
        }

        var ind = -1;

        for (var i = startInd; i < endInd; i++)
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

        if (startInd < endInd)
            spec.AddProp(expr.Substring(startInd, endInd - startInd));

        return spec;
    }

    private static AggregateFn GetAggregateFn(string str)
    {
        if (str.StartsWith(MAX))
            return AggregateFn.Max;

        if (str.StartsWith(MIN))
            return AggregateFn.Min;

        if (str.StartsWith(AVG))
            return AggregateFn.Avg;

        return str.StartsWith(SUM) ? AggregateFn.Sum : AggregateFn.None;
    }
}