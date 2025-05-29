using System;

namespace AVS.CoreLib.Extensions;

/// <summary>
/// Generic decimal extensions
/// </summary>
public static class DecimalExtensions
{
    #region Math extensions
    public static decimal Abs(this decimal value)
    {
        return value < 0 ? -value : value;
    }

    public static decimal Divide(this decimal value, decimal divider, decimal @default = 0, int? roundDecimals = null)
    {
        if (divider == 0)
            return @default;

        return (value / divider).Round(roundDecimals);
    }

    public static decimal SafeDivide(this decimal value, decimal divider, decimal @default = 0, int? roundDecimals = null)
    {
        if (divider == 0)
            return @default;

        return (value / divider).Round(roundDecimals);
    }

    /// <summary>
    /// returns fraction of the value e.g. Fraction(500, 0.5%) => 2.5
    /// </summary>
    /// <param name="value"></param>
    /// <param name="percent"></param>
    /// <returns></returns>
    public static decimal Fraction(this decimal value, decimal percent)
    {
        return value * percent / 100;
    }

    /// <summary>
    /// Returns a square root of a specified number, using double precision internally.
    /// Note: Precision may be lost due to conversion to and from double.
    /// </summary>
    public static decimal Sqrt(this decimal value)
    {
        var result = Math.Sqrt((double)value);

        if (double.IsInfinity(result) || double.IsNaN(result))
            throw new OverflowException("Result is not a valid decimal value.");

        return (decimal)result;
    }

    /// <summary>
    /// Raises a decimal number to a decimal power using double precision internally.
    /// Note: Precision may be lost due to conversion to and from double.
    /// </summary>
    public static decimal Pow(this decimal value, decimal pow)
    {
        var result = Math.Pow((double)value, (double)pow);

        if (double.IsInfinity(result) || double.IsNaN(result))
            throw new OverflowException("Result is not a valid decimal value.");

        return (decimal)result;
    }

    /// <summary>
    /// Raises a decimal number to a decimal power using double precision internally.
    /// Note: Precision may be lost due to conversion to and from double.
    /// </summary>
    public static decimal Pow(this decimal value, double pow)
    {
        var result = Math.Pow((double)value, pow);

        if (double.IsInfinity(result) || double.IsNaN(result))
            throw new OverflowException("Result is not a valid decimal value.");

        return (decimal)result;
    } 
    #endregion

    #region Round extensions

    public static decimal Round(this decimal value, int decimals, decimal step = 1m)
    {
        if (decimals >= 0)
            return decimal.Round(value / step, decimals, MidpointRounding.AwayFromZero) * step;

        // Handle negative decimals by scaling the number
        var factor = (decimal)Math.Pow(10, -decimals) * step;
        return decimal.Round(value / factor, 0, MidpointRounding.AwayFromZero) * factor;
    }

    public static decimal Round(this decimal value, int? roundDecimals = null, int extraPrecision = 0, int minPrecision = 0, decimal step =1m)
    {
        var dec = (roundDecimals ?? value.GetRoundDecimals()) + extraPrecision;
        if (minPrecision > 0 && dec < minPrecision)
            dec = minPrecision;

        return Round(value / step, dec) * step;
    }

    public static decimal RoundUp(this decimal value, int decimals, decimal step =1m)
    {
        var k = (decimal)Math.Pow(10, decimals);
        return step * decimal.Ceiling((value * k) / step) / k;
    }

    public static decimal RoundUp(this decimal number, decimal round)
    {
        var n = decimal.Ceiling((number + round) / round) - 1;
        return (n * round);
    }

    public static decimal RoundDown(this decimal number, decimal round)
    {
        var n = decimal.Floor((number + round) / round) - 1;
        return (n * round);
    }

    public static decimal RoundDown(this decimal value, int decimals, decimal step = 1m)
    {
        var k = (decimal)Math.Pow(10, decimals);
        return step * decimal.Floor((value * k) / step) / k;
    }

    public static bool IsRound(this decimal value, int decimals = 0)
    {
        return decimals == 0
            ? value % 1m == 0
            : value.Round(decimals) == value;
    }

    #endregion

    #region IsEqual exntesions
    public static bool IsEqual(this decimal value, decimal valueToCompare, decimal tolerance)
    {
        var equal = decimal.Abs(value - valueToCompare) <= tolerance;
        return equal;
    }

    public static bool IsEqual(this decimal value, decimal valueToCompare, decimal tolerance, out decimal diff)
    {
        diff = decimal.Abs(value - valueToCompare);
        var equal = diff <= tolerance;
        return equal;
    }

    public static bool IsNotEqual(this decimal value, decimal valueToCompare, decimal tolerance)
    {
        return decimal.Abs(value - valueToCompare) > tolerance;
    }

    public static bool IsLessThanOrEqual(this decimal value, decimal valueToCompare, decimal tolerance)
    {
        return value <= valueToCompare || Math.Abs(value - valueToCompare) <= tolerance;
    }

    public static bool IsGreaterThanOrEqual(this decimal value, decimal valueToCompare, decimal tolerance)
    {
        return value >= valueToCompare || Math.Abs(value - valueToCompare) <= tolerance;
    } 
    #endregion
}

/// <summary>
/// Additional decimal extensions which are a bit of a special purpose 
/// </summary>
public static class DecimalAdditionalExtensions
{
    /// <summary>
    /// +1 digit of precision comparing to <see cref="DecimalExtensions.Round(decimal,int?,int,int, decimal)"/>
    /// </summary>
    public static decimal RoundPrice(this decimal value, int? roundDecimals = null, int extraPrecision = 0, int minPrecision = 0)
    {
        var dec = (roundDecimals ?? value.GetPriceRoundDecimals()) + extraPrecision;
        if (minPrecision > 0 && dec < minPrecision)
            dec = minPrecision;
        return decimal.Round(value, dec, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// +2 digits of precision comparing to <see cref="DecimalExtensions.Round(decimal,int?,int,int, decimal)"/>
    /// </summary>
    public static decimal RoundPrecise(this decimal value, int? roundDecimals = null, int extraPrecision = 0, int minPrecision = 0)
    {
        var dec = (roundDecimals ?? value.GetRoundDecimals()) + 2 + extraPrecision;
        if (minPrecision > 0 && dec < minPrecision)
            dec = minPrecision;
        return decimal.Round(value, dec, MidpointRounding.AwayFromZero);
    }

    public static int GetPriceRoundDecimals(this decimal price)
    {
        return price switch
        {
            >= 10_000 => 0,
            >= 1_000 => 1,
            >= 100 => 2,
            >= 10 => 3,
            >= 1 => 4,
            >= 0.1M => 5,
            >= 0.01m => 6,
            >= 0.001m => 7,
            _ => 8,
        };
    }

    /// <summary>
    /// determines number of meaningful digits based on price value
    /// </summary>
    public static int GetRoundDecimals(this decimal value)
    {
        return value.Abs() switch
        {
            > 100 => 2,
            > 1 => 3,
            > 0.1m => 4,
            > 0.01m => 5,
            > 0.001m => 6,
            _ => 8
        };
    }

    public static bool WithinRange(this decimal value, (decimal from, decimal to) range, bool inclusiveRange = true)
    {
        return inclusiveRange
            ? value >= range.from && value <= range.to
            : value > range.from && value < range.to;
    }

    public static decimal GetGreaterValue(this (decimal, decimal) tuple)
    {
        return tuple.Item1 >= tuple.Item2 ? tuple.Item1 : tuple.Item2;
    }

    public static decimal GetSmallerValue(this (decimal, decimal) tuple)
    {
        return tuple.Item1 <= tuple.Item2 ? tuple.Item1 : tuple.Item2;
    }

    public static decimal GetGreaterValue(this decimal value, decimal value2)
    {
        return value >= value2 ? value : value2;
    }

    public static decimal GetSmallerValue(this decimal value, decimal value2)
    {
        return value <= value2 ? value : value2;
    }
}