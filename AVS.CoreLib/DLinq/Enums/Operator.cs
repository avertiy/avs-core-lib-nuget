using AVS.CoreLib.DLinq.Specs.Conditioning;

namespace AVS.CoreLib.DLinq.Enums;

/// <summary>
/// Comparison operators for <see cref="ComparisonSpec"/>
/// </summary>
public enum Operator
{
    Undefined = 0,
    Gt = 1,
    Lt = 2,
    Eq = 3,
    EqEq = 4,
    GtOrEq = 5,
    LtOrEq = 6,
    Not = 7,
    Is = 8,
    In = 10,
    Between = 11
}


public static class OperatorExtensions
{
    public static string ToExprString(this Operator op)
    {
        return op switch
        {
            Operator.Gt => ">",
            Operator.Lt => "<",
            Operator.GtOrEq => ">=",
            Operator.LtOrEq => "<=",
            Operator.Eq => "=",
            Operator.EqEq => "==",
            Operator.Is => "IS",
            Operator.Not => "NOT",
            Operator.In => "IN",
            Operator.Between => "BETWEEN",
            _ => string.Empty
        };
    }

    public static Operator ParseOperator(this string str)
    {
        return str switch
        {
            ">" => Operator.Gt,
            "<" => Operator.Lt,
            ">=" => Operator.GtOrEq,
            "<=" => Operator.LtOrEq,
            "=" => Operator.Eq,
            "==" => Operator.EqEq,
            "IS" => Operator.Is,
            "NOT" => Operator.Not,
            "IN" => Operator.In,
            "BETWEEN" => Operator.Between,
            _ => Operator.Undefined
        };
    }
}