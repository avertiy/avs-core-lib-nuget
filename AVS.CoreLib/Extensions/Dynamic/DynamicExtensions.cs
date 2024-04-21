using System;
using AVS.CoreLib.Lambdas;

namespace AVS.CoreLib.Extensions.Dynamic;

public static class DynamicExtensions
{
    /// <summary>
    /// dynamic cast  Func{object,object}(x => (TargetType)x);
    /// </summary>
    public static object Cast<T>(this T obj, Type targetType)
    {
        var fn = LambdaBag.Lambdas.CastTo<T>(targetType);
        return fn(obj!);
    }
    /// <summary>
    /// cast from abstraction to concrete type (use case json serialization IBar bar - serializer need a concrete type)
    /// </summary>
    public static object ToConcreteType<T>(this T obj)
    {
        var fn = LambdaBag.Lambdas.CastTo<T>(obj!.GetType());
        return fn(obj);
    }
}