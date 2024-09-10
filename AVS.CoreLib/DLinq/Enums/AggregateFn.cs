namespace AVS.CoreLib.DLinq.Enums;

public enum AggregateFn
{
    Undefined = 0,
    Avg,
    Max,
    Min,
    Sum
}

public static class AggregateFnExtensions
{
    public static string Format(this AggregateFn fn, string arg)
    {
        if (fn == AggregateFn.Undefined)
            return arg;

        return $"{fn:G}({arg})";
    }
}