using System;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public interface IPowerConsolePrinter : IXPrinter
    {
    }

    public class PowerConsolePrinter : XPrinter, IPowerConsolePrinter
    {
        public PowerConsolePrinter(TextWriter writer, ColorMode mode = ColorMode.AnsiCodes) : base(writer, mode)
        {
        }
    }
}