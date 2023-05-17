using System;
using AVS.CoreLib.Extensions.Stringify;
using System.Linq;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Guards;

public static class GuardMustBeExtensions
{
    #region Equal

    public static void Equal<T>(this IMustBeGuardClause guardClause, T arg1, T arg2, string? message = null)
    {
        if (arg1 == null || arg2 == null || !arg1.Equals(arg2))
            throw new ArgumentException(message ?? $"'{arg1}' expected to be equal '{arg2}'");
    }

    public static void EnumEqual<T>(this IMustBeGuardClause guardClause, T arg1, T arg2, string? message = null) where T: Enum
    {
        if (!arg1.Equals(arg2))
            throw new ArgumentException(message ?? $"'{arg1}' expected to be equal '{arg2}'");
    }

    
    public static void Equal(this IMustBeGuardClause guardClause, string? str1, string? str2, string? message = null)
    {
        if (str1 == null || str2 == null || !str1.Equals(str2))
            throw new ArgumentException(message ?? $"'{str1}' expected to be equal '{str2}'");
    }

    public static void Equal(this IMustBeGuardClause guardClause, double value, double valueToCompare, double tolerance = 0.001, string name = "argument")
    {
        if (!value.IsEqual(valueToCompare, tolerance))
            throw new ArgumentException($"{name} must be equal to {valueToCompare}");
    }

    public static void Equal(this IMustBeGuardClause guardClause, decimal value, decimal valueToCompare, decimal tolerance = 0.001m, string name = "argument")
    {
        if (!value.IsEqual(valueToCompare, tolerance))
            throw new ArgumentException($"{name} must be equal to {valueToCompare}");
    } 
    #endregion

    #region WithinRange
    public static void WithinRange(this IMustBeGuardClause guardClause, int value, int from, int to, bool inclusiveRange = true,
    string name = "argument")
    {
        if (inclusiveRange)
        {
            if (value < from || value > to)
                throw new ArgumentOutOfRangeException($"{name} is out of range [{from};{to}]");
        }
        else
        {
            if (value <= from || value >= to)
                throw new ArgumentOutOfRangeException($"{name} is out of range ({from};{to})");
        }
    }

    public static void WithinRange(this IMustBeGuardClause guardClause, double value, double from, double to, bool inclusiveRange = true, string name = "argument")
    {
        if (inclusiveRange)
        {
            if (value < from || value > to)
                throw new ArgumentOutOfRangeException($"{name} is out of range [{from};{to}]");

        }
        else
        {
            if (value <= from || value >= to)
                throw new ArgumentOutOfRangeException($"{name} is out of range ({from};{to})");
        }
    }

    public static void WithinRange(this IMustBeGuardClause guardClause, int value, (int from, int to) range, bool inclusiveRange = true, string name = "argument")
    {
        if (inclusiveRange)
        {
            if (value < range.from || value > range.to)
                throw new ArgumentOutOfRangeException($"{name} is out of range [{range.from};{range.to}]");
        }
        else
        {
            if (value <= range.from || value >= range.to)
                throw new ArgumentOutOfRangeException($"{name} is out of range ({range.from};{range.to})");
        }
    }
    #endregion

    #region GreaterThan / GreaterThanOrEqual
    public static void GreaterThan(this IMustBeGuardClause guardClause, int value, int number = 0, string name = "argument")
    {
        if (value <= number)
            throw new ArgumentException($"{name} must be greater than {number}");
    }

    public static void GreaterThan(this IMustBeGuardClause guardClause, double value, double number = 0, string name = "argument")
    {
        if (value <= number)
            throw new ArgumentException($"{name} must be greater than {number}");
    }

    public static void GreaterThan(this IMustBeGuardClause guardClause, decimal value, decimal number = 0, string name = "argument")
    {
        if (value <= number)
            throw new ArgumentException($"{name} must be greater than {number}");
    }

    public static void GreaterThanOrEqual(this IMustBeGuardClause guardClause, int value, int number = 0, string name = "argument")
    {
        if (value < number)
            throw new ArgumentException($"{name} must be greater or equal to {number}");
    }

    public static void GreaterThanOrEqual(this IMustBeGuardClause guardClause, double value, double number = 0, string name = "argument")
    {
        if (value < number)
            throw new ArgumentException($"{name} must be greater or equal to {number}");
    }

    public static void GreaterThanOrEqual(this IMustBeGuardClause guardClause, decimal value, decimal number = 0, string name = "argument")
    {
        if (value < number)
            throw new ArgumentException($"{name} must be greater or equal to {number}");
    } 
    #endregion

    #region MustBePositive
    public static void MustBePositive(this IMustBeGuardClause guardClause, int value, string name = "argument")
    {
        if (value <= 0)
            throw new ArgumentException($"{name} must be positive number (value:{value})");
    }

    public static void MustBePositive(this IMustBeGuardClause guardClause, decimal value, string name = "argument")
    {
        if (value <= 0)
            throw new ArgumentException($"{name} must be positive number (value:{value})");
    }

    public static void MustBePositive(this IMustBeGuardClause guardClause, double value, string name = "argument")
    {
        if (value <= 0)
            throw new ArgumentException($"{name} must be positive number (value:{value})");
    }
    #endregion


    public static string OneOf(this IMustBeGuardClause guardClause, string value, params string[] values)
    {
        if (values.Contains(value))
            return value;

        throw new ArgumentOutOfRangeException($"{nameof(value)} `{value}` is not one of allowed values: {values.Stringify()}");
    }

    public static T OneOf<T>(this IMustBeGuardClause guardClause, T value, params T[] values)
    {
        if (values.Contains(value))
            return value;

        throw new ArgumentOutOfRangeException($"{nameof(value)} `{value}` is not one of allowed values: {string.Join(", ", values)}");
    }

}