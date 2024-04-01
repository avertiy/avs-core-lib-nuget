using System;
using System.Linq.Expressions;

namespace AVS.CoreLib.DLinq.LambdaSpec;

/// <summary>
/// represent lambda spec basic building block
/// </summary>
public interface ISpecItem
{
    Type? CastTo { get; set; }
    Type? ArgType { get; set; }
    string ToString(SpecView view);
    Expression GetValueExpr(Expression argExpr, Func<Expression, Type?> resolveType);
}