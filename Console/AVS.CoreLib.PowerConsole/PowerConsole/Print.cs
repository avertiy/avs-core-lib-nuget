﻿using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Printers2;

namespace AVS.CoreLib.PowerConsole
{
    /// <summary>
    /// PowerConsole provides various extension methods to console printing
    /// support color printing (via ansi codes <see cref="AnsiCodes"/>, and by switching colors <see cref="SwitchColorOutputWriter"/>
    /// support color tags <see cref="CTagProcessor"/>
    /// provide various print utility method like PrintTable, PrintArray, PrintKeyValue etc.
    /// </summary>
    /// <remarks>If you need more rich and extensive console frameworks check out links below </remarks>
    /// <seealso>https://github.com/Athari/CsConsoleFormat - advanced formatting of console output for .NET</seealso>
    /// <seealso>https://github.com/migueldeicaza/gui.cs - Terminal GUI toolkit for .NET</seealso>
    /// <seealso>http://elw00d.github.io/consoleframework/- cross-platform toolkit that allows to develop TUI applications using C# and based on WPF-like concepts</seealso>
    public static partial class PowerConsole
    {
        private static IPowerConsolePrinter2? _printer2;

        public static IPowerConsolePrinter2 Printer2
        {
            get => _printer2 ??= new PowerConsolePrinter2(System.Console.Out);
            set => _printer2 = value;
        }

        ///// <summary> 
        ///// Format string if any text preprocessors are configured
        ///// <see cref="X.Format(FormattableString,IFormatPreprocessor)"/>
        ///// <seealso cref="X.FormatPreprocessor"/>
        ///// </summary>
        ///// /// <remarks>
        ///// X.Format extends standard .NET string format modifiers with custom modifiers
        ///// (<see cref="Text.Formatters.CustomFormatter"/> which is base class to implement custom formatter, example implementation see PriceFormatter in AVS.CoreLib.Trading package)
        ///// usage example: X.Format($"order: {type:+} {symbol:Q}"); `+` and `Q` are custom modifiers
        ///// or PowerConsole.XFormat($"order: {type:+} {symbol:Q}");
        ///// </remarks>
        //public static string XFormat(FormattableString str)
        //{
        //    return Printer is XPrinter printer ? printer.XFormatInternal(str) : X.Format(str, X.FormatPreprocessor);
        //}

        /// <summary>
        /// switch color printing mode <see cref="ColorMode"/> 
        /// </summary>
        public static void SwitchColorMode(ColorMode mode)
        {
            Printer2.SwitchMode(mode);
        }

        public static void Print(string str, PrintOptions2 options = PrintOptions2.Default, Colors? colors = null)
        {
            Printer2.Print(str, options, colors);
        }
    }
}
