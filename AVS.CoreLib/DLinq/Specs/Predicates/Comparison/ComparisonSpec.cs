using System;
using System.Diagnostics;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Enums;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.DLinq.Specs.Predicates.Comparison;

/// <summary>
/// Represent a right part of comparison statement
/// e.g. x >= 1 the right part is >= 1
/// </summary>
[DebuggerDisplay("ComparisonSpec: {Op} {Arg} (raw: {Raw})")]
public class ComparisonSpec : SpecBase
{
    public string Arg { get; set; }

    public Operator Op { get; set; }

    public ComparisonSpec(Operator op, string arg)
    {
        Arg = arg;
        Op = op;
    }

    protected ComparisonSpec(Operator op)
    {
        Op = op;
        Arg = string.Empty;
    }

    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        var type = expression.Type;

        var obj = Parser.TryParse(Arg, type);
        var expr = ComparisonExpr(expression, obj, Op);
        return expr;
    }

    public override string GetCacheKey()
    {
        return ToString();
    }

    public override string ToString()
    {
        return $"{Op.ToExprString()} {Arg}";
    }

    private Expression ComparisonExpr(Expression expr, object? arg, Operator op)
    {
        var argExpr = Expression.Constant(arg);

        return op switch
        {
            Operator.Gt => Expression.GreaterThan(expr, argExpr),
            Operator.Lt => Expression.LessThan(expr, argExpr),
            Operator.GtOrEq => Expression.GreaterThanOrEqual(expr, argExpr),
            Operator.LtOrEq => Expression.LessThanOrEqual(expr, argExpr),
            Operator.Not => Expression.Not(Expression.Equal(expr, argExpr)),
            Operator.Is => Expression.Equal(expr, argExpr),
            Operator.EqEq => Expression.Equal(expr, argExpr),
            Operator.Eq => Expression.Equal(expr, argExpr),
            _ => throw new NotImplementedException($"Operator {op.ToExprString()} not supported by {GetType().Name}")
        };
    }

    public static ComparisonSpec Parse(string part1, string part2)
    {
        // A) comparison: >,<,== etc. e.g. A > 1
        // B) NULL-checks: A IS NULL, A NOT NULL
        // C) BETWEEN: A BETWEEN 10 AND 20
        // D) IN: A IN (1,2,3) 

        var op = part1.ParseOperator();

        if (op == Operator.Undefined)
            throw new InvalidExpression($"Invalid operator `{part1}`");

        if (op == Operator.Between)
            return BetweenSpec.Parse(part2);

        if (op == Operator.In)
            return OperatorInSpec.Parse(part2);

        var spec = new ComparisonSpec(op, part2);
        return spec;
    }
}