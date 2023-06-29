using System.Globalization;

namespace AVS.CoreLib.Extensions.Formatting;

public static class FormatExtensions
{
    public static string ToCurrencyString(this decimal value, int? precision = null)
    {
        var p = precision ?? value.GetPricePrecision();
        return value.ToString("C" + p, CultureInfo.CurrentCulture);
    }

    public static int GetPricePrecision(this decimal price)
    {
        return price switch
        {
            >= 10_000 => 0,
            >= 1_000 => 1,
            >= 100 => 2,
            >= 1 => 3,
            >= 0.1m => 4,
            >= 0.01m => 5,
            >= 0.001m => 6,
            >= 0.0001m => 7,
            _ => 8,
        };
    }
}