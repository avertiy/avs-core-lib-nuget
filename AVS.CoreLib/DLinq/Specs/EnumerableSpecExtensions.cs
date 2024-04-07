using System;
using System.Collections;
using System.Collections.Generic;
using AVS.CoreLib.Expressions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Specs;

public static class EnumerableSpecExtensions
{
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
        var key = $"Select<{typeof(T).GetReadableName()}> {spec.GetBody()} [mode: {ctx.Mode}]";
        var bag = LambdaBag.Lambdas;

        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        fn = SpecCompiler.BuildFn<T>(spec, ctx);
        bag[key] = fn;
        return fn;
    }
}