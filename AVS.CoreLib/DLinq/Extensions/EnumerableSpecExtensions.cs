using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AVS.CoreLib.DLinq.Enums;
using AVS.CoreLib.DLinq.Specs;
using AVS.CoreLib.DLinq.Specs.CompoundBlocks;
using AVS.CoreLib.Extensions.Enums;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.Lambdas;

[assembly: InternalsVisibleTo("AVS.CoreLib.Tests")]
namespace AVS.CoreLib.DLinq.Extensions;
internal static class EnumerableSpecExtensions
{
    public static IEnumerable Select<T>(this IEnumerable<T> source, ILambdaSpec spec, SelectMode mode = SelectMode.Default)
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

    private static Func<IEnumerable<T>, IEnumerable> GetSelectFn<T>(ILambdaSpec spec, LambdaContext ctx)
    {
        var key = $"Select<T>({spec.GetCacheKey()}) [mode: {ctx.Mode}]";
        var bag = LambdaBag.Lambdas;

        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        fn = SpecCompiler.BuildSelectFn<T>(spec, ctx);
        bag[key] = fn;
        return fn;
    }

    public static IEnumerable<T> Where<T>(this IEnumerable<T> source, ILambdaSpec spec, SelectMode mode = SelectMode.Default)
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
    
    private static Func<T, bool> GetPredicateFn<T>(ILambdaSpec spec, LambdaContext ctx)
    {
        var key = $"predicate: {spec.GetCacheKey()} [mode: {ctx.Mode}]";
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
        var key = $"OrderBy({spec.GetCacheKey()}, direction) [mode: {ctx.Mode}]";
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
        var key = $"ThenBy({spec.GetCacheKey()}, direction) [mode: {ctx.Mode}]";
        var bag = LambdaBag.Lambdas;

        if (bag.TryGetFunc(key, out Func<IOrderedEnumerable<T>, Sort, IOrderedEnumerable<T>>? fn))
            return fn!;

        fn = SpecCompiler.BuildThenByFn<T>(spec, ctx);
        bag[key] = fn;
        return fn;
    }
    #endregion
    
    //private static Func<T, TKey> GetSelectorFn<T, TKey>(ValueExprSpec spec, LambdaContext ctx)
    //{
    //    var key = $"selector: {spec.GetBody()} [mode: {ctx.Mode}]";
    //    var bag = LambdaBag.Lambdas;

    //    if (bag.TryGetFunc(key, out Func<T, TKey>? fn))
    //        return fn!;

    //    fn = SpecCompiler.BuildSelector<T, TKey>(spec, ctx);
    //    bag[key] = fn;
    //    return fn;
    //}
}