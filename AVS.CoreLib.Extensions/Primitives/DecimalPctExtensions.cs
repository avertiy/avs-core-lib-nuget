using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Extensions;

public static class DecimalPctExtensions
{
    /// <summary>
    /// Calculates the percentage in the pct (whole) representation
    /// <code>
    /// formula: value / mean * 100%
    /// e.g. 1 / 20 *100% => 5 (in pct representation means 5%)
    /// </code>
    /// </summary>
    public static decimal Pct(this decimal value, decimal mean, int? roundDecimals = null)
    {
        return mean == 0 ? 0 : (value / mean * 100m).Round(roundDecimals);
    }

    /// <summary>
    /// Calculates the percentage in the fraction representation
    /// <code>
    /// formula: value / mean
    /// e.g. 1 / 20 => 0.05m (in fraction representation means 5%)
    /// </code>
    /// </summary>
    public static decimal PctAsFraction(this decimal value, decimal mean, int? roundDecimals = null)
    {
        return mean == 0 ? 0 : (value / mean).Round(roundDecimals);
    }

    /// <summary>
    /// Converts a pct representation (5%) to its fractional equivalent (0.05m).
    /// </summary>
    public static decimal ToFraction(this decimal value, decimal lowerThreshold = 1m)
    {
        Guard.MustBe.Pct(value, lowerThreshold);
        return value / 100m;
    }

    /// <summary>
    /// Converts a fractional representation (0.05m) to pct (whole) representation (5%).
    /// </summary>
    public static decimal ToPct(this decimal value, decimal upperThreshold = 1m)
    {
        Guard.MustBe.Fraction(value, upperThreshold);
        return value * 100m;
    }

    /// <summary>
    /// Applies percentage to the price values
    /// <code>
    /// price = 100;
    /// price.ApplyPercent(+5.Percent());   // 105
    /// price.ApplyPercent(-5.Percent());   // 95
    /// </code>
    /// </summary>
    public static decimal ApplyPercent(this decimal value, decimal pct, int? roundDecimals = null)
    {
        return (value * (1 + pct / 100m)).Round(roundDecimals);
    }

    /// <summary>
    /// Adjusts the value by the specified fractional change.
    /// <code>
    /// formula: value * (1 + fraction)
    /// e.g. 100 * (1 + 0.05m) => 105
    /// </code>
    /// </summary>
    public static decimal AdjustByFraction(this decimal value, decimal fraction, int? roundDecimals = null)
    {
        return (value * (1 + fraction)).Round(roundDecimals);
    }
}