using System;
using System.Linq;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Conditions;
using AVS.CoreLib.DLinq.Specs;
using AVS.CoreLib.DLinq.Specs.CompoundBlocks;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.DLinq.Specs.Conditioning;

/// <summary>
/// Represent a simple condition: A > 1
/// </summary>
public class ConditionSpec : SpecBase
{
    public ConditionSpec(ValueExpr1Spec value, ComparisonSpec comparison)
    {
        Value = value;
        Comparison = comparison;
    }

    public ValueExpr1Spec Value { get; set; }

    public ComparisonSpec Comparison { get; set; }

    public override Expression BuildExpr(Expression expr, LambdaContext ctx)
    {
        throw new NotImplementedException();
    }

    public override string ToString(string arg, SpecView view)
    {
        return $"{Value.ToString(arg, view)} {Comparison.ToString(arg, view)}";
    }

    public static ConditionSpec Parse(string expr, Type argType)
    {
        Guard.Against.NullOrEmpty(expr);

        var parts = expr.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
            throw new DLinqException($"Invalid syntax `{expr}` - condition expression might have at least 3 parts");

        var compSpec = ComparisonSpec.Parse(parts[1], parts[2]);
        var valueSpec = ValueExpr1Spec.Parse(parts[0], argType);
        var spec = new ConditionSpec(valueSpec, compSpec);
        return spec;
    }
}

public abstract class ComparisonSpec : SpecBase
{
    public Operator Op { get; set; }

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
            return BetweenSpec.Parse(part2);

        var spec = new SimpleComparisonSpec(op, part2);
        return spec;
    }
}


public class SimpleComparisonSpec : ComparisonSpec
{

    public string Arg { get; set; }

    public SimpleComparisonSpec(Operator op, string arg)
    {
        Arg = arg;
        Op = op;
    }
    
    public override Expression BuildExpr(Expression expr, LambdaContext ctx)
    {
        throw new NotImplementedException();
    }

    public override string ToString(string arg, SpecView view)
    {
        return $"{Op.ToExprString()} {Arg}";
    }

    
}

public class BetweenSpec : ComparisonSpec
{
    public string Arg1 { get; set; }
    public string Arg2 { get; set; }

    public BetweenSpec(string arg1, string arg2)
    {
        Arg1 = arg1;
        Arg2 = arg2;
        Op = Operator.Between;
    }

    public override Expression BuildExpr(Expression expr, LambdaContext ctx)
    {
        throw new NotImplementedException();
    }

    public override string ToString(string arg, SpecView view)
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

public class OperatorInSpec : ComparisonSpec
{
    public OperatorInSpec(params string[] args)
    {
        Args = args;
        Op = Operator.In;
    }

    public string[] Args { get; set; }


    public override Expression BuildExpr(Expression expr, LambdaContext ctx)
    {
        throw new NotImplementedException();
    }

    public override string ToString(string arg, SpecView view)
    {
        return $"IN ({string.Join(',', Args)})";
    }

    public static OperatorInSpec Parse(string str)
    {
        var args = str.TrimStart('(').TrimEnd(')').Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
        return new OperatorInSpec(args);
    }
}