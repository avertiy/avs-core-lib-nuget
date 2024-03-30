using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AVS.CoreLib.Extensions.Reflection;

public static class ReflectionHelper
{
    public static bool IsNullable(Type type)
    { 
        return type is { IsValueType: true, IsGenericType: true } && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// You should cache this delegate, because constantly recompiling Linq expressions can be expensive
    /// </summary>
    public static Func<object> CreateDefaultConstructor(Type type)
    {
        // Create a new lambda expression with the NewExpression as the body.
        var lambda = Expression.Lambda<Func<object>>(Expression.New(type));
        // Compile our new lambda expression.
        return lambda.Compile();
    }

    public static Type ConstructList(Type item)
    {
        var generic = typeof(List<>);
        var type = generic.MakeGenericType(item);
        return type;
    }

    public static Type ConstructDictionary<TKey>(Type value)
    {
        var generic = typeof(Dictionary<,>);
        var type = generic.MakeGenericType(typeof(TKey), value);
        return type;
    }

    public static Type ConstructDictionary(Type key, Type value)
    {
        var generic = typeof(Dictionary<,>);
        var type = generic.MakeGenericType(key, value);
        return type;
    }

    public static MethodInfo ConstructStaticMethod(this Type type, string methodName, params Type[] typeArguments)
    {
        var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
        return method.MakeGenericMethod(typeArguments);
    }
}