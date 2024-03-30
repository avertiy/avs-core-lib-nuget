using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace AVS.CoreLib.DLinq0.LambdaSpec0;
/*
public abstract class LambdaSpec<T>
{
    public Type TypeArg { get; set; } = null!;
    public Type ValueType { get; set; } = null!;
    public SpecType Type { get; set; }
    public List<ISpecItem> Items { get; set; } = null!;
    public int Count => Items.Count;
    public int Skip { get; set; }
    public int Take { get; set; }
}

//seems confusion with lexemes
public enum SpecType
{
    /// <summary>
    /// `*` or `.*`
    /// </summary>
    Any = 0,
    /// <summary>
    /// simple property e.g. `close` or comma-separated e.g. `close,high`
    /// </summary>
    Simple = 1,
    /// <summary>
    /// contains indexing [] operator, e.g. `prop[1],prop[2]`
    /// </summary>
    Indexed = 2,
    /// <summary>
    /// contains indexing by key [] operator , e.g. `prop["key1"],prop["key2"]`
    /// </summary>
    Keyed = 3,
    /// <summary>
    /// contains `.` dot operator to reference inner properties e.g. `prop1.inner,prop2.inner`
    /// </summary>
    Inner = 4,
    /// <summary>
    /// contains complex statements with . and indexing operators e.g. `prop["key1"].value` or `prop[0].inner.value["key1"]`
    /// </summary>
    Compound = 5
}

public interface ISpecItem
{
    string Name { get; set; }
    Expression GetExpression(Expression paramExpr, Type? castTo = null);
}
*/