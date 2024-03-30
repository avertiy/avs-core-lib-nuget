using System.Linq.Expressions;

namespace AVS.CoreLib.DLinq.LambdaSpec;

/// <summary>
/// represent lambda spec basic building block
/// </summary>
public interface ISpecItem
{
    string ToString(SpecView view);
    Expression GetValueExpr(Expression argExpr);
}