using System;

namespace AVS.CoreLib.Extensions;

public static class DoubleExtensions
{
    public static bool IsRound(this double value, int decimals = 0)
    {
        return decimals == 0
            ? value % 1 == 0
            : value.Round(decimals) == value;
    }

    public static int GetDecimalPlaces(this decimal number)
    {
        var rest = number % 1;
        if (rest == 0)
            return 0;

        rest = rest.Abs();
        //let's say rest=0.151
        var decimalPlaces = 0;
        while (rest > 0)
        {
            decimalPlaces++;
            rest *= 10;
            rest -= (int)rest;
        }
        return decimalPlaces;
    }

    public static double Round(this double value, int decimals)
    {
        return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// determines number of meaningful digits based on price value
    /// </summary>
    public static int GetRoundDecimals(this double value)
    {
        return value.Abs() switch
        {
            > 100 => 2,
            //> 10 => 3,
            > 1 => 3,
            > 0.1 => 4,
            > 0.01 => 5,
            > 0.001 => 6,
            _ => 8
        };
    }

    public static double Round(this double value, int? roundDecimals = null)
    {
        var dec = roundDecimals ?? GetRoundDecimals(value);
        return Math.Round(value, dec, MidpointRounding.AwayFromZero);
    }

    public static double RoundUp(this double value, int decimals)
    {
        var k = Math.Pow(10, decimals);
        return Math.Ceiling((value * k)) / k;
    }
    public static double RoundDown(this double value, int decimals)
    {
        var k = Math.Pow(10, decimals);
        return Math.Floor((value * k)) / k;
    }

    public static double Abs(this double value)
    {
        if (value < 0)
            value = value * -1;
        return value;
    }

    public static bool IsEqual(this double value, double valueToCompare, double tolerance)
    {
        var equal = Math.Abs(value - valueToCompare) <= tolerance;
        return equal;
    }

    public static bool IsNotEqual(this double value, double valueToCompare, double tolerance)
    {
        var equal = Math.Abs(value - valueToCompare) <= tolerance;
        return equal;
    }


}