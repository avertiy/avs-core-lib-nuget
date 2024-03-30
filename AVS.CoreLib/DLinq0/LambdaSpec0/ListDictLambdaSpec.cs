using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq;

namespace AVS.CoreLib.DLinq0.LambdaSpec0;

public class ListLambdaSpec<T>
{
    public Type TypeArg { get; set; }
    public Type ValueType { get; set; }

    public List<SpecItem> Items { get; set; }

    public int Count => Items.Count;

    public ListLambdaSpec(int capacity, Type typeArg)
    {
        Items = new List<SpecItem>(capacity);
        ValueType = typeof(object);
        TypeArg = typeArg;
    }
}

public class ListDictLambdaSpec<T>
{
    public Type TypeArg { get; set; }
    public Type ValueType { get; set; }
    public List<DictSpecItem> Items { get; set; }

    public int Count => Items.Count;

    public ListDictLambdaSpec(int capacity, Type typeArg)
    {
        Items = new List<DictSpecItem>(capacity);
        ValueType = typeof(object);
        TypeArg = typeArg;
    }

    public string GetCacheKey(string methodName)
    {
        var propsStr = string.Join(",", Items.Select(x => x.ItemKey));
        var key = $"source.{methodName}<{typeof(T).Name},{ValueType.Name}>(spec: {{ {propsStr}], {TypeArg.Name} }})";
        return key;
    }

    public void AddItem(DictSpecItem item)
    {
        Items.Add(item);
    }

    public override string ToString()
    {
        var propsStr = string.Join(",", Items.Select(x => x.ItemKey));
        return $"[{ValueType.Name}, [{propsStr}], {TypeArg.Name}]";
    }


    public Expression<Func<T, Dictionary<string, object>>> BuildLambda()
    {
        // Define parameter expression for the input of the lambda expression
        var sourceType = typeof(T);
        var paramExpr = Expression.Parameter(sourceType, "x");
        var paramCastExpr = Expression.Convert(paramExpr, TypeArg ?? sourceType);

        var dictType = typeof(Dictionary<string, object>);
        var addMethodInfo = dictType.GetMethod("Add")!;

        // Declare variable: Dictionary<string,TValue> dict
        var dictExpr = Expression.Variable(dictType, "dict");
        var dictConstructor = dictType.GetConstructor(new[] { typeof(int) })!;

        //var dict = new Dictionary<string, TValue>(props.Length);
        var dictAssignExpr = Expression.Assign(dictExpr, Expression.New(dictConstructor, Expression.Constant(Count)));

        var objType = typeof(object);
        var list = new List<Expression>(Count + 2) { dictAssignExpr };

        list.AddRange(Items.Select(x => x.GetExpression(dictExpr, addMethodInfo, paramCastExpr, objType)));

        list.Add(dictExpr);

        // Combine the add expressions into a block
        var blockExpr = Expression.Block(dictType, new[] { dictExpr }, list);
        var lambda = Expression.Lambda<Func<T, Dictionary<string, object>>>(blockExpr, paramExpr);
        return lambda;
    }

    public Expression<Func<T, Dictionary<string, TValue>>> BuildLambda<TValue>()
    {
        // Define parameter expression for the input of the lambda expression
        var sourceType = typeof(T);
        var paramExpr = Expression.Parameter(sourceType, "x");
        var paramCastExpr = Expression.Convert(paramExpr, TypeArg ?? sourceType);

        var dictType = typeof(Dictionary<string, TValue>);
        var addMethodInfo = dictType.GetMethod("Add")!;

        // Declare variable: Dictionary<string,TValue> dict
        var dictExpr = Expression.Variable(dictType, "dict");
        var dictConstructor = dictType.GetConstructor(new[] { typeof(int) })!;

        //var dict = new Dictionary<string, TValue>(props.Length);
        var dictAssignExpr = Expression.Assign(dictExpr, Expression.New(dictConstructor, Expression.Constant(Count)));

        var list = new List<Expression>(Count + 2) { dictAssignExpr };

        list.AddRange(Items.Select(x => x.GetExpression(dictExpr, addMethodInfo, paramCastExpr)));

        list.Add(dictExpr);

        // Combine the add expressions into a block
        var blockExpr = Expression.Block(dictType, new[] { dictExpr }, list);
        var lambda = Expression.Lambda<Func<T, Dictionary<string, TValue>>>(blockExpr, paramExpr);
        return lambda;
    }
}