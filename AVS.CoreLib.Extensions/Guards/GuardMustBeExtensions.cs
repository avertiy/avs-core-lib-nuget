using System;
using System.Linq;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Stringify;

namespace AVS.CoreLib.Guards;

public static class GuardMustBeExtensions
{
    public static void BeTrue(this IMustBeGuardClause guardClause, bool arg, string? message = null)
    {
        if (!arg)
            throw new ArgumentException(message ?? $"'{arg}' must be True");
    }

    public static void BeFalse(this IMustBeGuardClause guardClause, bool arg, string? message = null)
    {
        if (arg)
            throw new ArgumentException(message ?? $"'{arg}' must be False");
    }

    #region Equal

    public static void Equal<T>(this IMustBeGuardClause guardClause, T arg1, T arg2, string? message = null)
    {
        if (arg1 == null || arg2 == null || !arg1.Equals(arg2))
            throw new ArgumentException(message ?? $"'{arg1}' must be equal '{arg2}'");
    }

    public static void EnumEqual<T>(this IMustBeGuardClause guardClause, T arg1, T arg2, string? message = null) where T : Enum
    {
        if (!arg1.Equals(arg2))
            throw new ArgumentException(message ?? $"'{arg1}' must be equal '{arg2}'");
    }

    public static void EnumEqualOrDefault<T>(this IMustBeGuardClause guardClause, T arg1, T arg2, T @default, string? message = null) where T : Enum
    {
        if (!arg1.Equals(arg2) && !arg1.Equals(@default))
            throw new ArgumentException(message ?? $"'{arg1}' must be equal '{arg2}' or default `{@default}`");
    }


    public static void Equal(this IMustBeGuardClause guardClause, string? str1, string? valueToCompare, string? message = null)
    {
        if (str1 == null || valueToCompare == null || !str1.Equals(valueToCompare))
            throw new ArgumentException(message ?? $"'{str1}' must be equal '{valueToCompare}'");
    }

    public static void Equal(this IMustBeGuardClause guardClause, double arg, double valueToCompare, double tolerance = 0.001, string? message = null)
    {
        if (!arg.IsEqual(valueToCompare, tolerance))
            throw new ArgumentException(message ?? $"{arg} must be equal to {valueToCompare}");
    }

    public static void Equal(this IMustBeGuardClause guardClause, decimal arg, decimal valueToCompare, string? message = null)
    {
        if (arg != valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be equal to {valueToCompare}");
    }

    public static void Equal(this IMustBeGuardClause guardClause, decimal arg, decimal valueToCompare, decimal tolerance, string? message = null)
    {
        if (!arg.IsEqual(valueToCompare, tolerance))
            throw new ArgumentException(message ?? $"{arg} must be equal to {valueToCompare}");
    }
    #endregion

    #region NotEqual

    public static void NotEqual(this IMustBeGuardClause guardClause, string? arg, string? valueToCompare, string? message = null)
    {
        if (arg == valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be not equal to {valueToCompare}");
    }

    public static void NotEqual(this IMustBeGuardClause guardClause, int arg, int valueToCompare, string? message = null)
    {
        if (arg == valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be not equal to {valueToCompare}");
    }

    public static void NotEqual(this IMustBeGuardClause guardClause, decimal arg, decimal valueToCompare, string? message = null)
    {
        if (arg == valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be not equal to {valueToCompare}");
    }

    public static void NotEqual(this IMustBeGuardClause guardClause, decimal arg, decimal valueToCompare, decimal tolerance, string? message = null)
    {
        if (arg.IsEqual(valueToCompare, tolerance))
            throw new ArgumentException(message ?? $"{arg} must be not equal to {valueToCompare}");
    }

    public static void NotEqual(this IMustBeGuardClause guardClause, double arg, double valueToCompare, double tolerance, string? message = null)
    {
        if (arg.IsEqual(valueToCompare, tolerance))
            throw new ArgumentException(message ?? $"{arg} must be not equal to {valueToCompare}");
    }

    public static void NotEqual<T>(this IMustBeGuardClause guardClause, T arg1, T arg2, string? message = null)
    {
        if (arg1 == null || arg1.Equals(arg2))
            throw new ArgumentException(message ?? $"'{arg1}' must be not equal to '{arg2}'");
    }


    #endregion

    #region WithinRange
    public static void WithinRange(this IMustBeGuardClause guardClause, int arg, int from, int to, bool inclusiveRange = true,
    string? message = null)
    {
        if (inclusiveRange)
        {
            if (arg < from || arg > to)
                throw new ArgumentOutOfRangeException(message ?? $"{arg} must be within range [{from};{to}]");
        }
        else
        {
            if (arg <= from || arg >= to)
                throw new ArgumentOutOfRangeException(message ?? $"{arg} must be within range ({from};{to})");
        }
    }

    public static void WithinRange(this IMustBeGuardClause guardClause, double arg, double from, double to, bool inclusiveRange = true, string? message = null)
    {
        if (inclusiveRange)
        {
            if (arg < from || arg > to)
                throw new ArgumentOutOfRangeException(message ?? $"{arg} must be within range [{from};{to}]");

        }
        else
        {
            if (arg <= from || arg >= to)
                throw new ArgumentOutOfRangeException(message ?? $"{arg} must be within range ({from};{to})");
        }
    }

    public static void WithinRange(this IMustBeGuardClause guardClause, int arg, (int from, int to) range, bool inclusiveRange = true, string? message = null)
    {
        if (inclusiveRange)
        {
            if (arg < range.from || arg > range.to)
                throw new ArgumentOutOfRangeException(message ?? $"{arg} must be within range [{range.from};{range.to}]");
        }
        else
        {
            if (arg <= range.from || arg >= range.to)
                throw new ArgumentOutOfRangeException(message ?? $"{arg} must be within range ({range.from};{range.to})");
        }
    }

    public static void WithinRange(this IMustBeGuardClause guardClause, decimal arg, (decimal from, decimal to) range, bool inclusiveRange = true, string? message = null)
    {
        if (inclusiveRange)
        {
            if (arg < range.from || arg > range.to)
                throw new ArgumentOutOfRangeException(message ?? $"{arg} must be within range [{range.from};{range.to}]");
        }
        else
        {
            if (arg <= range.from || arg >= range.to)
                throw new ArgumentOutOfRangeException(message ?? $"{arg} must be within range ({range.from};{range.to})");
        }
    }

    #endregion

    #region GreaterThan / GreaterThanOrEqual
    public static void GreaterThan(this IMustBeGuardClause guardClause, int arg, int valueToCompare = 0, string? message = null)
    {
        if (arg <= valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be greater than {valueToCompare}");
    }

    public static void GreaterThan(this IMustBeGuardClause guardClause, double arg, double valueToCompare = 0, string? message = null)
    {
        if (arg <= valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be greater than {valueToCompare}");
    }

    public static void GreaterThan(this IMustBeGuardClause guardClause, decimal arg, decimal valueToCompare = 0, string? message = null)
    {
        if (arg <= valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be greater than {valueToCompare}");
    }

    public static void GreaterThanOrEqual(this IMustBeGuardClause guardClause, int arg, int valueToCompare = 0, string? message = null)
    {
        if (arg < valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be greater or equal to {valueToCompare}");
    }

    public static void GreaterThanOrEqual(this IMustBeGuardClause guardClause, double arg, double valueToCompare = 0, string? message = null)
    {
        if (arg < valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be greater or equal to {valueToCompare}");
    }

    public static void GreaterThanOrEqual(this IMustBeGuardClause guardClause, decimal arg, decimal valueToCompare = 0, string? message = null)
    {
        if (arg < valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be greater or equal to {valueToCompare}");
    }
    #endregion

    #region LessThan / LessThanOrEqual
    public static void LessThan(this IMustBeGuardClause guardClause, int arg, int valueToCompare = 0, string? message = null)
    {
        if (arg >= valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be less than {valueToCompare}");
    }

    public static void LessThan(this IMustBeGuardClause guardClause, double arg, double valueToCompare = 0, string? message = null)
    {
        if (arg >= valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be less than {valueToCompare}");
    }

    public static void LessThan(this IMustBeGuardClause guardClause, decimal arg, decimal valueToCompare = 0, string? message = null)
    {
        if (arg >= valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be less than {valueToCompare}");
    }

    public static void LessThanOrEqual(this IMustBeGuardClause guardClause, int arg, int valueToCompare = 0, string? message = null)
    {
        if (arg > valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be less or equal to {valueToCompare}");
    }

    public static void LessThanOrEqual(this IMustBeGuardClause guardClause, double arg, double valueToCompare = 0, string? message = null)
    {
        if (arg > valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be less or equal to {valueToCompare}");
    }

    public static void LessThanOrEqual(this IMustBeGuardClause guardClause, decimal arg, decimal valueToCompare = 0, string? message = null)
    {
        if (arg > valueToCompare)
            throw new ArgumentException(message ?? $"{arg} must be less or equal to {valueToCompare}");
    }
    #endregion

    #region MustBe Positive

    public static void Positive(this IMustBeGuardClause guardClause, int arg, string? message = null)
    {
        if (arg <= 0)
            throw new ArgumentException(message ?? $"{arg} must be positive");
    }

    public static void Positive(this IMustBeGuardClause guardClause, long arg, string? message = null)
    {
        if (arg <= 0)
            throw new ArgumentException(message ?? $"{arg} must be positive");
    }

    public static void Positive(this IMustBeGuardClause guardClause, decimal arg, string? message = null)
    {
        if (arg <= 0)
            throw new ArgumentException(message ?? $"{arg} must be positive");
    }

    public static void Positive(this IMustBeGuardClause guardClause, double arg, string? message = null)
    {
        if (arg <= 0)
            throw new ArgumentException(message ?? $"{arg} must be positive");
    }

    #endregion

    #region MustBe Negative

    public static void Negative(this IMustBeGuardClause guardClause, int arg, string? message = null)
    {
        if (arg >= 0)
            throw new ArgumentException(message ?? $"{arg} must be negative");
    }

    public static void Negative(this IMustBeGuardClause guardClause, decimal arg, string? message = null)
    {
        if (arg >= 0)
            throw new ArgumentException(message ?? $"{arg} must be negative");
    }

    public static void Negative(this IMustBeGuardClause guardClause, double arg, string? message = null)
    {
        if (arg >= 0)
            throw new ArgumentException(message ?? $"{arg} must be negative");
    }

    #endregion

    #region OneOf
    public static string OneOf(this IMustBeGuardClause guardClause, string arg, string[] values, string? message = null)
    {
        if (values.Contains(arg))
            return arg;

        throw new ArgumentOutOfRangeException(message ?? $"`{arg}` must be one of [{values.Stringify()}]");
    }

    public static string OneOf(this IMustBeGuardClause guardClause, string arg, params string[] values)
    {
        if (values.Contains(arg))
            return arg;

        throw new ArgumentOutOfRangeException($"`{arg}` must be one of [{values.Stringify()}]");
    }

    public static T OneOf<T>(this IMustBeGuardClause guardClause, T arg, params T[] values)
    {
        if (values.Contains(arg))
            return arg;

        throw new ArgumentOutOfRangeException($"`{arg}` must be one of [{string.Join(", ", values)}]");
    }

    #endregion
}