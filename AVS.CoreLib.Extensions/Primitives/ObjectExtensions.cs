using System;
using System.Collections;

namespace AVS.CoreLib.Extensions;

public static class ObjectExtensions
{
    public static bool IsEmpty(this object? obj)
    {
        if(obj == null) return true;

        return obj switch
        {
            int i => i == 0,
            long l => l == 0,
            double d => d == 0,
            decimal dec => dec == 0,
            short s => s == 0,
            float f => f == 0,
            bool b => b == false,
            string s => string.IsNullOrEmpty(s),
            DateTime dt => DateTime.MinValue == dt,
            TimeSpan ts => ts == TimeSpan.MinValue,
            Array arr => arr.Length == 0,
            IList list => list.Count == 0,
            IDictionary dict => dict.Count == 0,
            Guid guid => Guid.Empty == guid,
            _ => false
        };
    }

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

    public static bool IsPrimitive<T>(this T obj)
    {
        return obj switch
        {
            char i => true,
            int i => true,
            long l => true,
            double d => true,
            decimal dec => true,
            short s => true,
            float f => true,
            _ => false
        };
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