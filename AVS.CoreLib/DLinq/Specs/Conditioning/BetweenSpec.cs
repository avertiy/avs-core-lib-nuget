using System;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Conditions;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.DLinq.Specs.Conditioning;

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

    public override string ToString(string arg, SpecView view = SpecView.Default)
    {
        return $"{Operator.Between.ToExprString()} {Arg} AND {Arg2}";
    }

    public override string ToString()
    {
        return $"{nameof(BetweenSpec)} {Op.ToExprString()} {Arg}";
    }
}