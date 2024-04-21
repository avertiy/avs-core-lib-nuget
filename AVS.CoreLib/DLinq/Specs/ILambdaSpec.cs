using System.Linq.Expressions;
using AVS.CoreLib.Lambdas;

namespace AVS.CoreLib.DLinq.Specs;

/// <summary>
/// LambdaSpec represent a spec that could be used to build a lambda expression, compile and cache the lambda delegate (selector, predicate etc.)
/// </summary>
public interface ILambdaSpec
{
    /// <summary>
    /// holds a raw expression string 
    /// </summary>
    string? Raw { get; set; }
    Expression BuildExpr(Expression expr, LambdaContext ctx);

    /// <summary>
    /// cache key is used to cache a compiled lambda expression in <see cref="LambdaBag.Lambdas"/>
    /// </summary>
    string GetCacheKey();
}