using System;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Printers2;
using AVS.CoreLib.Text.FormatProviders;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        /// <summary>
        /// Format string with X.Format <see cref="XFormatProvider"/> 
        /// </summary>
        /// <remarks>
        /// in case you use X.Format
        /// strings staring with @ symbol are treated as strings with expression(s) and processed by TextProcessor(s)
        /// </remarks>
        public static void PrintF(FormattableString str, PrintOptions2 options = PrintOptions2.Default, Colors? colors = null)
        {
            Printer2.Print(str, options, colors);
        }

        //public static void PrintF(FormattableString str, ColorPalette colorPalette)
        //{
        //    Printer2.Print(str, PrintOptions.FromColorPalette(colorPalette));
        //}

        //public static void PrintF(FormattableString str, params ConsoleColor[] colors)
        //{
        //    Printer.PrintF(str, PrintOptions.FromColors(colors));
        //}
        //public static void PrintF(int posX, int posY, FormattableString str, PrintOptions2 options = PrintOptions2.Default)
        //{
        //    var text = str.ToString();
        //    var rows = Regex.Matches(text, Environment.NewLine).Count;
        //    ClearRegion(posX, posY, rows);
        //    Printer2.Print(str, options);
        //    ClearLine();
        //}
    }
}
