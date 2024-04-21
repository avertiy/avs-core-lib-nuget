using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AVS.CoreLib.Guards;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.Lambdas;

/// <summary>
/// Expression helper class helps to build some of the expressions
/// </summary>
public static class Expr
{
    public static Expression Cast(this ParameterExpression paramExpr, Type? argType)
    {
        return argType == null ? paramExpr : Expression.Convert(paramExpr, argType);
    }

    /// <summary>
    /// Creates expression: x => ((Type)x).Prop
    /// </summary>
    public static Expression PropExpr<T>(PropertyInfo prop, Type? paramType)
    {
        // Define parameter expression for the input of the lambda expression
        var paramExpr = Expression.Parameter(typeof(T), "x");

        // Access the property specified by the PropertyInfo
        var propExpr = paramType == null
            ? Expression.Property(paramExpr, prop)
            : Expression.Property(Expression.Convert(paramExpr, paramType), prop);

        return propExpr;
    }

    public static TryExpression WrapInTryCatch(Expression expr)
    {
        if (expr.NodeType == ExpressionType.Try)
            return (TryExpression)expr;

        TryExpression tryExpr;

        if (expr.Type == typeof(void))
            tryExpr = Expression.TryCatch(
                // Try block: return x[key]
                Expression.Block(expr),
                // Catch block: return default / null
                Expression.Catch(typeof(Exception), Expression.Empty())
            );
        else
            tryExpr = Expression.TryCatch(
                // Try block: return x[key]
                Expression.Block(expr),
                // Catch block: return default / null
                Expression.Catch(typeof(Exception), Expression.Default(expr.Type))
            );

        return tryExpr;
    }

    /// <summary>
    /// Creates dictionary expression (determines value type based on expressions i.e. typed or object dictionary):
    /// <code>
    /// (i) object: x => XActivator.CreateDictionary(keys, new object[] {expr1, expr2, ...})
    /// (ii) typed: x => XActivator.CreateValueDictionary(keys, new [] {expr1, expr2, ...})
    /// </code>
    /// </summary>
    public static MethodCallExpression CreateDictionaryExpr(string[] keys, params Expression[] expressions)
    {
        Guard.Array.MustHaveAtLeast(expressions, 1);

        var useTypedDict = expressions.All(x => x.Type == expressions[0].Type);

        var method = useTypedDict
            ? XActivator.CreateDictionaryMethodInfo(expressions[0].Type)
            : XActivator.CreateDictionaryMethodInfo();

        var keysExpr = Expression.Constant(keys);

        var valuesExpr = useTypedDict
            ? Expression.NewArrayInit(expressions[0].Type, expressions)
            : Expression.NewArrayInit(typeof(object), expressions.Select(x => Expression.Convert(x, typeof(object))));

        var expr = Expression.Call(null, method, keysExpr, valuesExpr);
        return expr;
    }


    /// <summary>
    /// Creates dictionary expression (object or typed dictionary depends on props' PropertyType):
    /// (i) object: x => XActivator.CreateDictionary(keys, new object[] {expr1, expr2, ...})
    /// (ii) typed: x => XActivator.CreateValueDictionary(keys, new [] {expr1, expr2, ...})
    /// </summary>
    public static MethodCallExpression CreateDictionaryExpr<T>(Expression argExpr, PropertyInfo[] props)
    {
        Guard.Array.MustHaveAtLeast(props, 1);

        var keys = props.Select(x => x.Name).ToArray();
        var keysExpr = Expression.Constant(keys);
        var uniqueTypes = props.Select(x => x.PropertyType).Distinct().ToArray();

        Expression valuesArrExpr;
        MethodInfo method;
        if (uniqueTypes.Length == 1)
        {
            //typed dictionary
            method = XActivator.CreateDictionaryMethodInfo(uniqueTypes[0]);
            valuesArrExpr = Expression.NewArrayInit(uniqueTypes[0], props.Select(x => Expression.Property(argExpr, x)));
        }
        else
        {
            method = XActivator.CreateDictionaryMethodInfo();
            var objType = typeof(object);
            var expressions = props.Select(x => Expression.Convert(Expression.Property(argExpr, x), objType));
            valuesArrExpr = Expression.NewArrayInit(objType, expressions);
        }

        var callExpr = Expression.Call(null, method, keysExpr, valuesArrExpr);
        return callExpr;
    }
}