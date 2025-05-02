using System.Globalization;

namespace AVS.CoreLib.Extensions.Formatting;

public static class FormatExtensions
{
    public static string ToCurrencyString(this decimal value, int? precision = null)
    {
        var p = precision ?? value.GetPriceRoundDecimals();
        return value.ToString("C" + p, CultureInfo.CurrentCulture);
    }
}