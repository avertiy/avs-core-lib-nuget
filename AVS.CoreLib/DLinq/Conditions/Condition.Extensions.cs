using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.DLinq.Conditions;

public partial class Condition : ICondition
{
    public const string AND_TOKEN = "AND";
    public const string OR_TOKEN = "OR";

    public static ICondition OR(params string[] parts)
    {
        Guard.Array.MinLength(parts, 1);

        return parts.Length switch
        {
            1 => new Condition(parts[0]),
            2 => new BinaryCondition(parts[0], Op.OR, parts[1]),
            _ => new MultiCondition(Op.OR, parts)
        };
    }

    public static ICondition AND(params string[] parts)
    {
        Guard.Array.MinLength(parts, 1);

        return parts.Length switch
        {
            1 => new Condition(parts[0]),
            2 => new BinaryCondition(parts[0], Op.AND, parts[1].Trim()),
            _ => new MultiCondition(Op.AND, parts)
        };
    }

    public static ICondition Join(Op op, IEnumerable<ICondition> conditions)
    {
        var parts = conditions.Where(x => x != Condition.Empty).ToArray();
        return parts.Length switch
        {
            1 => parts[0],
            2 => new BinaryCondition(parts[0], op, parts[1]),
            _ => new MultiCondition(op, parts)
        };
    }

   
}

public static class ConditionExtensions
{
    internal static string[] SplitByOR(this string str)
    {
        return str.Split(Condition.OR_TOKEN, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
    }

    internal static string[] SplitByAND(this string str)
    {
        return str.Split(Condition.AND_TOKEN, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
    }
}