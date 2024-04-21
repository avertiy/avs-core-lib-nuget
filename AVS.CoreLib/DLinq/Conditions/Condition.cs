using AVS.CoreLib.DLinq.Specs;
using AVS.CoreLib.DLinq.Specs.Predicates;

namespace AVS.CoreLib.DLinq.Conditions;

/// <summary>
/// Represent a logical condition e.g. A >= 1
/// Conditions could be:
///  (i) unary <see cref="Condition"/> => <seealso cref="PredicateSpec"/>
///  (ii) binary <see cref="BinaryCondition"/> => <seealso cref="CompoundPredicateSpec"/>
///  (iii) multi <see cref="MultiCondition"/> => <seealso cref="CompoundPredicateSpec"/>
/// </summary>
public interface ICondition
{
    ILambdaSpec GetSpec(DLinqContext context);
}

public partial class Condition : ICondition
{
    public string Expr { get; set; }

    public static Condition Empty { get; } = new(string.Empty);

    public Condition(string expr)
    {
        Expr = expr.Trim();
    }

    public override string ToString()
    {
        return Expr;
    }

    public ILambdaSpec GetSpec(DLinqContext context)
    {
        return PredicateSpec.Parse(Expr, context);
    }
}

