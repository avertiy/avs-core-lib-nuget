using System;
using System.Collections;
using System.Collections.Generic;

namespace AVS.CoreLib.Extensions;

/// <summary>
/// Contains various useful object extensions
/// </summary>
public static class ObjectExtensions
{
    public static bool IsEmpty(this object? obj)
    {
        if (obj == null)
            return true;

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

    public static bool IsDefault(this object? obj)
    {
        // Null is the default value for reference types and nullable value types
        if (obj is null)
            return true;

        // Get the actual runtime type
        Type type = obj.GetType();

        // Create a default instance of that type and compare
        return obj.Equals(Activator.CreateInstance(type));
    }


    public static bool IsDefault<T>(this T value)
    {
        return EqualityComparer<T>.Default.Equals(value, default);
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
        return obj is int or long or short or double or decimal or float or byte;
    }

    public static bool IsBoolean(this object obj)
    {
        return obj is bool;
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
            bool f => true,
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
            int i => i == 0 ? 0 : i > 0 ? 1 : -1,
            long l => l == 0 ? 0 : l > 0 ? 1 : -1,
            double d => d == 0 ? 0 : d > 0 ? 1 : -1,
            decimal dec => dec == 0 ? 0 : dec > 0 ? 1 : -1,
            short s => s == 0 ? 0 : s > 0 ? 1 : -1,
            float f => f == 0 ? 0 : f > 0 ? 1 : -1,
            _ => 0
        };
    }
}