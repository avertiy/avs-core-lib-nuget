using System;
using System.IO;
using AVS.CoreLib.PowerConsole.Enums;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public interface IPowerConsolePrinter : IXPrinter
    {
    }

    public class PowerConsolePrinter : XPrinter, IPowerConsolePrinter
    {
        internal static DateTime SystemTime => DateTime.Now;
        public PowerConsolePrinter(TextWriter writer, ColorMode mode = ColorMode.AnsiCodes) : base(writer, mode)
        {
        }
    }
}