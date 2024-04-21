using AVS.CoreLib.DLinq.Conditions;

namespace AVS.CoreLib.DLinq.Enums;

/// <summary>
/// Conditional operator to combine <see cref="ICondition"/>, <seealso cref="BinaryCondition"/>
/// </summary>
public enum Op
{
    Undefined = 0,
    AND = 1,
    OR = 2,
    NOT = 3,
}