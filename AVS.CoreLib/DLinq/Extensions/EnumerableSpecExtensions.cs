using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AVS.CoreLib.DLinq.Specs;
using AVS.CoreLib.DLinq.Specs.CompoundBlocks;
using AVS.CoreLib.Expressions;
using AVS.CoreLib.Extensions.Enums;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.Extensions.Linq;
[assembly: InternalsVisibleTo("AVS.CoreLib.Tests")]
namespace AVS.CoreLib.DLinq.Extensions;
internal static class EnumerableSpecExtensions
{
    public static IEnumerable Execute<T>(this ValueExprSpec spec, IEnumerable<T> source, SelectMode mode = SelectMode.Default)
    {
        if (spec.Fn > 0)
            return new[] { source.Aggregate(spec, mode) };

        return source.Select(spec, mode);
    }

    public static IEnumerable Execute<T>(this MultiValueExprSpec spec, IEnumerable<T> source, SelectMode mode = SelectMode.Default)
    {
        var count = spec.Count;
        if (count == 0)
            return source;

        if (count == 1)
            return spec.Items.First().Value.Execute(source, mode);

        if (spec.Items.All(x => x.Value.Fn > 0))
        {
            // multi aggregate functions case e.g MAX(Prop1),SUM(Prop2)
            var arr = source.ToArray();

            IEnumerable result;

            if (spec.Items.Any(x => x.Value.Alias != null))
                result = spec.Items.ToDictionary(x => x.Key, x => arr.Aggregate(x.Value, mode));
            else
                result = spec.Items.Values.Select(x => arr.Aggregate(x, mode)).ToList();

            return result;
        }

        return source.Select(spec, mode);
    }

    public static IEnumerable Select<T>(this IEnumerable<T> source, ISpec spec, SelectMode mode = SelectMode.Default)
    {
        var ctx = new LambdaContext() { Mode = mode, Source = source };
        var fn = GetSelectFn<T>(spec, ctx);
        try
        {
            return fn!.Invoke(source);
        }
        catch (Exception ex)
        {
            throw new DLinqException($"Invoke lambda failed - {ex.Message} [mode: {mode}]", ex, spec);
        }
    }

    private static Func<IEnumerable<T>, IEnumerable> GetSelectFn<T>(ISpec spec, LambdaContext ctx)
    {
        //{typeof(T).GetReadableName()}
        var key = $"Select<T>({spec.GetBody()}) [mode: {ctx.Mode}]";
        var bag = LambdaBag.Lambdas;

        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        fn = SpecCompiler.BuildSelectFn<T>(spec, ctx);
        bag[key] = fn;
        return fn;
    }

    public static IEnumerable<T> Where<T>(this IEnumerable<T> source, ISpec spec, SelectMode mode = SelectMode.Default)
    {
        var ctx = new LambdaContext() { Mode = mode, Source = source };
        var predicate = GetPredicateFn<T>(spec, ctx);
        try
        {
            return source.Where(predicate);
        }
        catch (Exception ex)
        {
            throw new DLinqException($"source.Where<{typeof(T).GetReadableName()}>({spec}) [mode: {mode}] failed - {ex.Message}", ex, spec);
        }
    }
    
    private static Func<T, bool> GetPredicateFn<T>(ISpec spec, LambdaContext ctx)
    {
        var key = $"predicate: {spec.GetBody()} [mode: {ctx.Mode}]";
        var bag = LambdaBag.Lambdas;

        if (bag.TryGetFunc(key, out Func<T, bool>? fn))
            return fn!;

        fn = SpecCompiler.BuildPredicate<T>(spec, ctx);
        bag[key] = fn;
        return fn;
    }

    #region OrderBy & ThenBy
    public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> source, ValueExprSpec spec, Sort direction, SelectMode mode = SelectMode.Default)
    {
        var ctx = new LambdaContext() { Mode = mode, Source = source };
        var orderByFn = GetOrderByFn<T>(spec, ctx);
        try
        {
            return orderByFn.Invoke(source, direction);
        }
        catch (Exception ex)
        {
            throw new DLinqException($"source.OrderBy({spec}, {direction}) failed - {ex.Message}", ex, spec);
        }
    }

    private static Func<IEnumerable<T>, Sort, IEnumerable<T>> GetOrderByFn<T>(ValueExprSpec spec, LambdaContext ctx)
    {
        var key = $"OrderBy({spec.GetBody()}, direction) [mode: {ctx.Mode}]";
        var bag = LambdaBag.Lambdas;

        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, Sort, IEnumerable<T>>? fn))
            return fn!;

        fn = SpecCompiler.BuildOrderByFn<T>(spec, ctx);
        bag[key] = fn;
        return fn;
    }

    public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> source, ValueExprSpec spec, Sort direction, SelectMode mode = SelectMode.Default)
    {
        var ctx = new LambdaContext() { Mode = mode, Source = source };
        var fn = GetThenByFn<T>(spec, ctx);
        try
        {
            return fn.Invoke(source, direction);
        }
        catch (Exception ex)
        {
            throw new DLinqException($"source.ThenBy({spec}, {direction}) failed - {ex.Message}", ex, spec);
        }
    }

    private static Func<IOrderedEnumerable<T>, Sort, IOrderedEnumerable<T>> GetThenByFn<T>(ValueExprSpec spec, LambdaContext ctx)
    {
        var key = $"ThenBy({spec.GetBody()}, direction) [mode: {ctx.Mode}]";
        var bag = LambdaBag.Lambdas;

        if (bag.TryGetFunc(key, out Func<IOrderedEnumerable<T>, Sort, IOrderedEnumerable<T>>? fn))
            return fn!;

        fn = SpecCompiler.BuildThenByFn<T>(spec, ctx);
        bag[key] = fn;
        return fn;
    }
    #endregion

    internal static object Aggregate<T>(this IEnumerable<T> source, ValueExprSpec spec, SelectMode mode = SelectMode.Default)
    {
        var ctx = new LambdaContext() { Mode = mode, Source = source };

        var fn = spec.Fn;
        var @delegate = GetSelectorFn<T>(spec, ctx);
        try
        {
            if (@delegate is Func<T, decimal> decSelector)
            {
                return fn switch
                {
                    AggregateFn.Max => source.Max(decSelector),
                    AggregateFn.Min => source.Min(decSelector),
                    AggregateFn.Sum => source.Sum(decSelector),
                    AggregateFn.Avg => source.Average(decSelector),
                    _ => throw new ArgumentOutOfRangeException(nameof(fn), fn, $"Not {fn} supported")
                };
            }

            if (@delegate is Func<T, double> doubleSelector)
            {
                return fn switch
                {
                    AggregateFn.Max => source.Max(doubleSelector),
                    AggregateFn.Min => source.Min(doubleSelector),
                    AggregateFn.Sum => source.Sum(doubleSelector),
                    AggregateFn.Avg => source.Average(doubleSelector),
                    _ => throw new ArgumentOutOfRangeException(nameof(fn), fn, $"Not {fn} supported")
                };
            }

            var selector = (Func<T, int>)@delegate;

            return fn switch
            {
                AggregateFn.Max => source.Max(selector),
                AggregateFn.Min => source.Min(selector),
                AggregateFn.Sum => source.Sum(selector),
                AggregateFn.Avg => source.Average(selector),
                _ => throw new ArgumentOutOfRangeException(nameof(fn), fn, $"Not {fn} supported")
            };
        }
        catch (Exception ex)
        {
            throw new DLinqException($"source.Aggregate({spec}, {fn}) failed - {ex.Message}", ex, spec);
        }
    }

    private static Delegate GetSelectorFn<T>(ValueExprSpec spec, LambdaContext ctx)
    {
        var key = $"selector: {spec.GetBody()} [mode: {ctx.Mode}]";
        var bag = LambdaBag.Lambdas;

        if (bag.ContainsKey(key))
            return bag[key];

        var fn = SpecCompiler.BuildAggregateFnSelector<T>(spec, ctx);
        bag[key] = fn;
        return fn;
    }

    //private static decimal AggregateDecimal<T>(this IEnumerable<T> source, ValueExprSpec spec, AggregateFn fn, SelectMode mode = SelectMode.Default)
    //{
    //    var ctx = new LambdaContext() { Mode = mode, Source = source };

    //    var selector = GetSelectorFn<T, decimal>(spec, ctx);
    //    try
    //    {
    //        return source.Aggregate(selector, fn);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new DLinqException($"source.Aggregate({spec}, {fn}) failed - {ex.Message}", ex, spec);
    //    }
    //}

    private static Func<T, TKey> GetSelectorFn<T, TKey>(ValueExprSpec spec, LambdaContext ctx)
    {
        var key = $"selector: {spec.GetBody()} [mode: {ctx.Mode}]";
        var bag = LambdaBag.Lambdas;

        if (bag.TryGetFunc(key, out Func<T, TKey>? fn))
            return fn!;

        fn = SpecCompiler.BuildSelector<T, TKey>(spec, ctx);
        bag[key] = fn;
        return fn;
    }
}