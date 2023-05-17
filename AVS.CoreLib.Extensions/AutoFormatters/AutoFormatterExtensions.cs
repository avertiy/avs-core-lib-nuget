using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace AVS.CoreLib.Extensions.AutoFormatters;

public static class AutoFormatterExtensions
{
    internal static IAutoFormatter AddBaseFormatters(this IAutoFormatter formatter)
    {
        formatter.AddFormatter<decimal>(x => x.Round().ToString(CultureInfo.CurrentCulture));
        formatter.AddFormatter<double>(x => x.Round().ToString(CultureInfo.CurrentCulture));
        formatter.AddFormatter<DateTime>(x => x.ToString("g",CultureInfo.CurrentCulture));
        return formatter;
    }

    internal static IAutoFormatter AddFinancialFormatters(this IAutoFormatter formatter)
    {
        formatter.AddFormatterByKeyword<decimal>("price", x => x.ToString("C", CultureInfo.CurrentCulture));
        formatter.AddFormatterByKeyword<decimal>("total", x => x.ToString("C", CultureInfo.CurrentCulture));
        formatter.AddFormatterByKeyword<decimal>("pnl", x => x.ToString("C", CultureInfo.CurrentCulture));
        formatter.AddFormatterByKeyword<decimal>("roe", x => x.ToString("P", CultureInfo.CurrentCulture));
        return formatter;
    }

    public static IEnumerable<string> FormatAll(this IAutoFormatter formatter, IEnumerable source)
    {
        foreach (var obj in source)
        {
            yield return formatter.Format(obj);
        }
    }

    public static IEnumerable<string> FormatAll<T>(this AutoFormatter formatter, IEnumerable<T> source)
    {
        var type = typeof(T);
        var key = type.Name;
        var format = formatter.Formatters.GetFormatterOrDefault(key, AutoFormatter.DEFAULT_FORMATTER);
        foreach (var obj in source)
        {
            yield return format(obj!);
        }
    }

    public static void AddFormatter<T>(this AutoFormatter formatter,string key, Func<T, string> format)
    {
        formatter.Formatters.Register(key, x => format((T)x));
    }
}