using System;
using System.Diagnostics;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Enums;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.DLinq.Specs.Predicates.Comparison;

/// <summary>
/// represent specification to for between comparison operator
/// <code>
/// expr BETWEEN 10 AND 50 is translated into an expression: x => x &gt;=10 and &lt;=50
/// </code>
/// </summary>
[DebuggerDisplay("BetweenSpec: {Arg} AND {Arg2}")]
public class BetweenSpec : ComparisonSpec
{
    public string Arg2 { get; set; }

    public BetweenSpec(string arg1, string arg2) : base(Operator.Between, arg1)
    {
        Arg2 = arg2;
    }

    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        var type = expression.Type;

        var obj = Parser.TryParse(Arg, type);
        var obj2 = Parser.TryParse(Arg2, type);

        var arg1Expr = Expression.Constant(obj);
        var arg2Expr = Expression.Constant(obj2);

        var leftExpr = Expression.GreaterThanOrEqual(expression, arg1Expr);
        var rightExpr = Expression.LessThanOrEqual(expression, arg2Expr);
        var expr = Expression.And(leftExpr, rightExpr);
        return expr;
    }

    public static BetweenSpec Parse(string str)
    {
        var args = str.Split("AND", StringSplitOptions.RemoveEmptyEntries);

        if (args.Length != 2)
            throw new InvalidExpression("BETWEEN operator must have 2 arguments: Value BETWEEN A AND B", str);

        return new BetweenSpec(args[0], args[1]);
    }

    public override string ToString()
    {
        return $"{Op.ToExprString()} {Arg} AND {Arg2}";
    }
}