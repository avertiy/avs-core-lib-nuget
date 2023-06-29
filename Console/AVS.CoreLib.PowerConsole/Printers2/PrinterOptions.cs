using System;
using System.Globalization;

namespace AVS.CoreLib.PowerConsole.Printers2
{
    public class PrinterOptions
    {
        public string? TimeFormat { get; set; }
        public Func<FormattableString, string> Format { get; set; } = str => str.ToString(CultureInfo.CurrentCulture);
        public static PrinterOptions Default { get; set; } = new PrinterOptions() { TimeFormat = "HH:mm:ss" };
    }
}