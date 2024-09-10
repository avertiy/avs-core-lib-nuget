using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq;
using AVS.CoreLib.DLinq.Enums;
using AVS.CoreLib.DLinq.Specs;
using AVS.CoreLib.DLinq.Specs.BasicBlocks;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.Lambdas;

namespace AVS.CoreLib.DLinq.Specs.CompoundBlocks;

/// <summary>
/// Represent a lambda specification single field (value) selector
/// e.g. x => x.close or more complex: x => x.prop[0]['key1'].value 
/// Contain one or more building blocks: <see cref="PropSpec"/>, <see cref="IndexSpec"/>, <see cref="KeySpec"/>
/// </summary>
[DebuggerDisplay("ValueExprSpec: {ArgType.Name} {Raw} [parts #{Count}]")]
public class ValueExprSpec : SpecBase, ILambdaSpec
{
    public AggregateFn Fn { get; set; }
    public string Name { get; set; } = null!;
    public string? Alias { get; set; }
    /// <summary>
    /// if alias ends with `!` the value expression itself will not be included in select
    /// e.g. bag[SMA(21)] as sma!, sma.Distance, sma.Value => source.Select(x=> { Distance = bag[SMA(21)].Distance, Value = bag[SMA(21)].Value})
    /// so bag[SMA(21)] object itself is omitted
    /// </summary>
    public bool Shortcut { get; set; }

    public List<PropSpec> Parts { get; private set; } = new();
    public int Count => Parts.Count;

    public required Type ArgType { get; set; }

    public Type? ReturnType { get; set; }

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
        var shortcutName = Parts[0].Name;
        Expression expr;

        if (shortcutName != null && ctx.Expressions.ContainsKey(shortcutName))
            expr = ctx.Expressions[shortcutName];
        else
        {
            expr = expression.Type == ArgType ? expression : Expression.Convert(expression, ArgType);
            expr = Parts[0].BuildExpr(expr, ctx);
        }

        for (var i = 1; i < Parts.Count; i++)
            expr = Parts[i].BuildExpr(expr, ctx);


        if (ReturnType != null)
            expr = Expression.Convert(expr, ReturnType);

        if (Alias != null)
            ctx.Expressions[Alias] = expr;

        if (ctx.Mode.HasFlag(SelectMode.Safe))
            expr = Expr.WrapInTryCatch(expr);

        return expr;
    }

    public override string ToString()
    {
        var str = "x";
        for (var i = 0; i < Parts.Count; i++)
            str = Parts[i].Format(str);

        str = ReturnType == null ? str : $"({ReturnType.GetReadableName()}){str}";

        str = Alias != null
            ? $"{Fn.Format(str)} as {(Shortcut ? Alias + "!" : Alias)}"
            : Fn.Format(str);

        return str;
    }

    public override string GetCacheKey()
    {
        var expr = ToString();
        var typeName = ArgType.GetReadableName();
        var key = $"{typeName} x => {expr}";
        return key;
    }

    public void ApplyShortcut(ValueExprSpec shortcut)
    {
        if (Parts[0].Name != shortcut.Alias)
            throw new InvalidOperationException($"{Parts[0].Name} != {shortcut.Alias}");

        // apply shortcut i.e. replace Parts[0] with shortcut.Parts
        var parts = new List<PropSpec>(shortcut.Parts);

        for (var i = 1; i < Parts.Count; i++)
            parts.Add(Parts[i]);

        Parts = parts;
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
        var expr = valueExpr.TrimStart(' ', '.');

        //if(expr.StartsWith('!'))

        var startInd = 0;
        //if (valueExpr.StartsWith("x."))
        //    startInd = 2;
        var endInd = expr.Length;

        var fn = GetAggregateFn(expr);

        var spec = new ValueExprSpec() { Fn = fn, ArgType = type, Raw = expr };

        var aliasInd = expr.IndexOf(AS, startInd, StringComparison.InvariantCultureIgnoreCase);

        if (aliasInd > -1)
        {
            // prop[sma] as sma!
            spec.Alias = expr.Substring(aliasInd + AS.Length).Trim();

            if (spec.Alias.EndsWith('!'))
            {
                spec.Shortcut = true;
                spec.Alias = spec.Alias.Substring(0, spec.Alias.Length - 1);
            }
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
        {
            var propExpr = expr.Substring(startInd, endInd - startInd);
            spec.AddProp(propExpr);
        }

        spec.Name = GetName(spec);

        return spec;
    }

    private static string GetName(ValueExprSpec spec)
    {
        if (spec.Alias != null)
            return spec.Alias;

        var parts = spec.Parts.Select(x => x.GetKey()).ToArray();

        var name = parts[0];

        if (parts.Length > 1)
        {
            var ind = Array.FindLastIndex(parts, x => char.IsLetter(x[0]));
            if (ind > 0 && parts[ind].ToLowerInvariant().Either("value", "item", "props"))
                ind--;
            name = string.Join('_', parts.Skip(ind));
        }

        if (spec.Fn > 0 && !char.IsLetter(name[0]))
            return spec.Fn.ToString("G").ToUpperInvariant();

        //return name;
        return spec.Fn.Format(name);
    }

    private static AggregateFn GetAggregateFn(string str)
    {
        if (str.StartsWith(MAX))
            return AggregateFn.Max;

        if (str.StartsWith(MIN))
            return AggregateFn.Min;

        if (str.StartsWith(AVG))
            return AggregateFn.Avg;

        return str.StartsWith(SUM) ? AggregateFn.Sum : AggregateFn.Undefined;
    }
}

public static class ValueExprSpecExtensions
{
    public static IEnumerable Execute<T>(this ValueExprSpec spec, IEnumerable<T> source, SelectMode mode = SelectMode.Default)
    {
        if (spec.Fn > 0)
            return new[] { source.Aggregate(spec, mode) };

        return source.Select(spec, mode);
    }

    internal static object Aggregate<T>(this IEnumerable<T> source, ValueExprSpec spec, SelectMode mode = SelectMode.Default)
    {
        var ctx = new LambdaContext() { Mode = mode, Source = source };

        var fn = spec.Fn;
        var @delegate = GetSelectorForAggregateFn<T>(spec, ctx);
        try
        {
            if (@delegate is Func<T, decimal> decSelector)
                return fn switch
                {
                    AggregateFn.Max => source.Max(decSelector),
                    AggregateFn.Min => source.Min(decSelector),
                    AggregateFn.Sum => source.Sum(decSelector),
                    AggregateFn.Avg => source.Average(decSelector),
                    _ => throw new ArgumentOutOfRangeException(nameof(fn), fn, $"Fn `{fn}` not supported")
                };

            if (@delegate is Func<T, double> doubleSelector)
                return fn switch
                {
                    AggregateFn.Max => source.Max(doubleSelector),
                    AggregateFn.Min => source.Min(doubleSelector),
                    AggregateFn.Sum => source.Sum(doubleSelector),
                    AggregateFn.Avg => source.Average(doubleSelector),
                    _ => throw new ArgumentOutOfRangeException(nameof(fn), fn, $"Fn `{fn}` not supported")
                };

            var selector = (Func<T, int>)@delegate;

            return fn switch
            {
                AggregateFn.Max => source.Max(selector),
                AggregateFn.Min => source.Min(selector),
                AggregateFn.Sum => source.Sum(selector),
                AggregateFn.Avg => source.Average(selector),
                _ => throw new ArgumentOutOfRangeException(nameof(fn), fn, $"Fn `{fn}` not supported")
            };
        }
        catch (Exception ex)
        {
            throw new DLinqException($"Aggregate extension failed - {ex.Message} [spec: {spec}, mode: {mode}]", ex, spec);
        }
    }

    private static Delegate GetSelectorForAggregateFn<T>(ValueExprSpec spec, LambdaContext ctx)
    {
        var key = $"{spec.GetCacheKey()} [mode: {ctx.Mode}]";

        var bag = LambdaBag.Lambdas;

        if (bag.ContainsKey(key))
            return bag[key];

        var fn = SpecCompiler.BuildAggregateFnSelector<T>(spec, ctx);
        bag[key] = fn;
        return fn;
    }
}