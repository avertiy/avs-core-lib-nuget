using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AVS.CoreLib.DLinq.Enums;
using AVS.CoreLib.Expressions;
using AVS.CoreLib.DLinq.Extensions;
using AVS.CoreLib.DLinq.Specs.CompoundBlocks;

namespace AVS.CoreLib.DLinq.Specs.LambdaSpecs;

/// <summary>
/// Represent lambda specification with multiple value expressions <see cref="ValueExprSpec"/>
/// Selector 
/// Contain one or more <see cref="ValueExprSpec"/>
/// </summary>
public class MultiValueExprSpec : CompoundSpec<ValueExprSpec>, ILambdaSpec
{
    public MultiValueExprSpec(int capacity) : base(capacity)
    {
    }

    public MultiValueExprSpec(IList<ValueExprSpec> items) : base(items.Count)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    public bool Contains(string name)
    {
        return Items.Any(x => x.Shortcut == false && x.Name == name);
    }

    public void Add(ValueExprSpec item)
    {
        if (Contains(item.Name))
            item.Name = ResolveCollision(item.Name);

        Items.Add(item);
    }

    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        switch (Items.Count)
        {
            case 0:
                return expression;
            case 1:
                return Items.First().BuildExpr(expression, ctx);
            default:
            {
                var keys = new List<string>(Items.Count);
                var expressions = new List<Expression>(Items.Count);
                var sameType = true;

                foreach (var item in Items)
                {
                    var key = item.Name;
                    var valueExpr = item.BuildExpr(expression, ctx);

                    if(item.Alias != null || !ctx.Expressions.ContainsKey(key))
                        ctx.Expressions[key] = valueExpr;

                    if (item.Shortcut)
                        continue;

                    keys.Add(key);
                    expressions.Add(valueExpr);

                    if (sameType && expressions.Count > 1 && expressions[0].Type != valueExpr.Type)
                        sameType = false;
                }

                var expr = Expr.CreateDictionaryExpr(keys.ToArray(), expressions.ToArray());
                return expr;
                }
        }
    }

    public string GetCacheKey()
    {
        var sb = new StringBuilder(Items.Count * 25);
        sb.Append('{');
        foreach (var item in Items)
        {
            sb.Append('{');
            sb.Append(item.Name);
            sb.Append(":");
            sb.Append(item.GetCacheKey());
            sb.Append("}, ");
        }

        if (sb.Length > 2)
            sb.Length -= 2;

        sb.Append('}');
        return sb.ToString();
    }

    protected override IEnumerable<Type> IterateArgTypes()
    {
        return Items.Select(x => x.ArgType);
    }
    

    private string ResolveCollision(string key)
    {
        var i = 1;
        while (Contains(key + "_" + i))
            i++;

        return key + "_" + i;
    }
}

internal static class MultiValueExprSpecExtensions
{
    public static IEnumerable Execute<T>(this MultiValueExprSpec spec, IEnumerable<T> source, SelectMode mode = SelectMode.Default)
    {
        var items = spec.Items;

        if (items.Count == 0 || items.All(x => x.Shortcut))
            return source;

        if (items.Any(x => x.Fn > 0))
            return spec.ExecuteAggregation(source, mode);

        return source.Select(spec, mode);
    }

    private static IEnumerable ExecuteAggregation<T>(this MultiValueExprSpec spec, IEnumerable<T> source,
        SelectMode mode)
    {
        // Filter out shortcuts and items with no aggregation function
        var items = spec.Items.Where(item => item is { Shortcut: false, Fn: > 0 }).ToList();

        if (items.Count == 0)
            return source;

        // Apply shortcuts
        foreach (var item in items)
        {
            var shortcut = spec.Items.FirstOrDefault(x => x.Alias != null && x.Alias == item.Parts[0].Name);
            if (shortcut != null)
                item.ApplyShortcut(shortcut);
        }

        // multi aggregates case
        var arr = source.ToArray();
        var result = items.ToDictionary(x => x.Name, x => arr.Aggregate(x, mode));
        return result;
    }
}