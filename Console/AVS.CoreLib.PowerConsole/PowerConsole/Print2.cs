using System.Collections.Generic;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        public static void PrintColorStrings(params ColorString[] messages)
        {
            Printer.Print(messages);
        }

        public static void Print(IEnumerable<ColorString> messages)
        {
            Printer.Print(messages);
        }

        public static void Print(ColorMarkupString str, PrintOptions? options = null)
        {
            Printer.Print(str, options ?? DefaultOptions);
        }
    }
}
