using System;
using System.Linq;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Conditions;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.DLinq.LambdaSpec.Conditioning;

/// <summary>
/// Represent a simple condition: A > 1
/// </summary>
public class ConditionSpec : Spec, ISpecItem
{
    public ConditionSpec(ValueExprSpec value, Spec comparison)
    {
        Value = value;
        Comparison = comparison;
    }

    public ValueExprSpec Value { get; set; }

    public Spec Comparison { get; set; }

    protected override Expression BuildValueExpr(Expression argExpr, Func<Expression, Type?> resolveType)
    {
        throw new NotImplementedException();
    }

    public override string ToString(SpecView view)
    {
        return $"{Value.ToString(view)} {Comparison.ToString(view)}";
    }

    public static ConditionSpec Parse(string expr)
    {
        Guard.Against.NullOrEmpty(expr);

        var parts = expr.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
            throw new DLinqException($"Invalid syntax `{expr}` - condition expression might have at least 3 parts");

        var compSpec = ComparisonSpec.Parse(parts[1], parts[2]);
        var valueSpec = ValueExprSpec.Parse(parts[0]);
        var spec = new ConditionSpec(valueSpec, compSpec);
        return spec;
    }
}


public class ComparisonSpec : Spec
{
    public Operator Op { get; set; }

    public string Arg { get; set; }

    public ComparisonSpec(Operator op, string arg)
    {
        Arg = arg;
        Op = op;
    }

    protected override Expression BuildValueExpr(Expression argExpr, Func<Expression, Type?> resolveType)
    {
        throw new NotImplementedException();
    }

    public override string ToString(SpecView view)
    {
        return $"{Op.ToExprString()} {Arg}";
    }

    public static Spec Parse(string part1, string part2)
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
            return BetweenSpec.Parse(part2);

        var spec = new ComparisonSpec(op, part2);
        return spec;
    }
}

public class BetweenSpec : Spec
{
    public string Arg1 { get; set; }
    public string Arg2 { get; set; }

    public BetweenSpec(string arg1, string arg2)
    {
        Arg1 = arg1;
        Arg2 = arg2;
    }

    protected override Expression BuildValueExpr(Expression argExpr, Func<Expression, Type?> resolveType)
    {
        throw new NotImplementedException();
    }

    public override string ToString(SpecView view)
    {
        return $"{Operator.Between.ToExprString()} {Arg1} AND {Arg2}";
    }

    public static BetweenSpec Parse(string str)
    {
        var args = str.Split("AND", StringSplitOptions.RemoveEmptyEntries);

        if (args.Length != 2)
            throw new InvalidExpression("BETWEEN operator must have 2 arguments: Value BETWEEN A AND B", str);

        return new BetweenSpec(args[0], args[1]);
    }
}

public class OperatorInSpec : Spec
{
    public OperatorInSpec(params string[] args)
    {
        Args = args;
    }

    public string[] Args { get; set; }

    protected override Expression BuildValueExpr(Expression argExpr, Func<Expression, Type?> resolveType)
    {
        throw new NotImplementedException();
    }

    public override string ToString(SpecView view)
    {
        return $"IN ({string.Join(',', Args)})";
    }

    public static OperatorInSpec Parse(string str)
    {
        var args = str.TrimStart('(').TrimEnd(')').Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
        return new OperatorInSpec(args);
    }
}