using System;

namespace AVS.CoreLib.Text.Formatters
{
    /// <summary>
    /// formats input "{DateTime.Now:!--Red d}" into string $$01/01/2020:--Red$
    /// - foreground color
    /// --background color 
    /// </summary>
    public class ColorFormatter : CustomFormatter
    {
        protected override string CustomFormat(string format, object arg, IFormatProvider formatProvider)
        {
            //format:-Color or --Color e.g -Red or --Red 
            var parts = format.Split(' ');

            var nextFormat = parts.Length > 1
                ? format.Substring(parts[0].Length + 1, format.Length - parts[0].Length - 1)
                : string.Empty;

            var value = DefaultFormat(nextFormat, arg, formatProvider);
            string str = $"$${value}:{parts[0]}$";
            return str;
        }

        protected override bool Match(string format) => format.StartsWith("-");
    }
}