using System;
namespace AVS.CoreLib.Text.Formatters
{
    /// <summary>
    /// if argument format starts with "!"  than in case argument is empty or (0 for numeric types, MinValue etc.) than returns string.Empty 
    /// </summary>
    public class NotEmptyFormatter : CustomFormatter
    {
        protected override string CustomFormat(string format, object arg, IFormatProvider formatProvider)
        {
            switch (arg)
            {
                case int i when i == 0:
                case decimal dec when Math.Abs(dec) == 0:
                case DateTime date when date == DateTime.MinValue:
                case string s when string.IsNullOrWhiteSpace(s):
                    return string.Empty;
                default:
                    {
                        if (format.Length == 1)
                            return arg.ToString();
                        var nextFormat = format.Substring(1, format.Length - 1).TrimStart();
                        return DefaultFormat(nextFormat, arg, formatProvider);
                    }
            }
        }

        protected override bool Match(string format) => format.StartsWith("!");
    }
}