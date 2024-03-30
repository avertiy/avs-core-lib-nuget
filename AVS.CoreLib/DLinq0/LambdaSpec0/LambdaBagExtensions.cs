using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AVS.CoreLib.DLinq;
using AVS.CoreLib.DLinq.LambdaSpec;

[assembly: InternalsVisibleTo("AVS.CoreLib.Tests")]
namespace AVS.CoreLib.DLinq0.LambdaSpec0;
internal static class LambdaBagExtensions
{
    internal static Func<IEnumerable<T>, IEnumerable> GetSelectFn<T>(this LambdaBag bag, ILambdaSpec spec)
    {
        var key = spec.GetCacheKey<T>();
        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        var lambda = spec.Build<T>();
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    internal static IEnumerable Execute<T>(this LambdaBag bag, ILambdaSpec spec, IEnumerable<T> source)
    {
        var fn = bag.GetSelectFn<T>(spec);
        return fn.Invoke(source);
    }
}