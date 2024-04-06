using AVS.CoreLib.DLinq.LambdaSpec;
using AVS.CoreLib.DLinq.LambdaSpec.Conditioning;

namespace AVS.CoreLib.DLinq.Conditions;


public interface ICondition
{
    Spec GetSpec();
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

    public Spec GetSpec()
    {
        return ConditionSpec.Parse(Expr);
    }
}

