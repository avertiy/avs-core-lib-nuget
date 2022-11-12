namespace AVS.CoreLib.Extensions;

public static class ObjectExtensions
{
    public static bool IsInteger(this object obj)
    {
        return obj is int or long or short;
    }

    public static bool IsFloating(this object obj)
    {
        return obj is double or decimal or float;
    }

    public static bool IsNumeric(this object obj)
    {
        return obj is int or long or short or double or decimal or float;
    }

    public static bool IsPositive(this object obj)
    {
        return obj switch
        {
            int i => i > 0,
            long l => l > 0,
            double d => d > 0,
            decimal dec => dec > 0,
            short s => s > 0,
            float f => f > 0,
            _ => false
        };
    }

    public static int GetSign(this object obj)
    {
        return obj switch
        {
            int i => i == 0 ? 0 : i > 0? 1: -1,
            long l => l == 0 ? 0 :l > 0 ? 1 : -1,
            double d => d == 0 ? 0 : d > 0 ? 1 : -1,
            decimal dec => dec == 0 ? 0 : dec > 0 ? 1 : -1,
            short s => s == 0 ? 0 : s > 0 ? 1 : -1,
            float f => f == 0 ? 0 : f > 0 ? 1 : -1,
            _ => 0
        };
    }
}