using System.Collections.Generic;
using AVS.CoreLib.PowerConsole.Printers2;
using AVS.CoreLib.PowerConsole.Printers2.Extensions;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        public static void PrintColorStrings(params ColorString[] messages)
        {
            Printer2.PrintMessages(messages);
        }

        public static void Print(IEnumerable<ColorString> messages)
        {
            Printer2.PrintMessages(messages);
        }

        public static void Print(ColorMarkupString str, PrintOptions2 options = PrintOptions2.Default)
        {
            Printer2.Print(str, options);
        }
    }
}
