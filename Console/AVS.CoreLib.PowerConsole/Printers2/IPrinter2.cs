using System;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Enums;

namespace AVS.CoreLib.PowerConsole.Printers2
{
    //work in process..
    //IPrinter2 will replace IPrinter
    /// <summary>
    /// Define printer base functionality
    /// </summary>
    public interface IPrinter2
    {
        /// <summary>
        /// direct writing to output
        /// </summary>
        void Write(string message);
        /// <summary>
        /// direct writing to output
        /// </summary>
        void WriteLine(string message);
        void WriteLine(MessageLevel level, string message);
        void WriteLine(bool voidMultipleNewLines = true);
        void Print(string message, PrintOptions2 options = PrintOptions2.Default);
        void Print(FormattableString str, PrintOptions2 options = PrintOptions2.Default);
    }

    /// <summary>
    /// Define printer base color functionality
    /// </summary>
    public interface IColorPrinter2 : IPrinter2
    {
        ColorMode Mode { get; }
        void Write(string message, Colors colors);
        void WriteLine(string message, Colors? colors = null);
        void Print(string message, PrintOptions2 options, Colors? colors);
        void Print(FormattableString str, PrintOptions2 options, Colors? colors);
        void Print(MessageLevel level, string message, PrintOptions2 options = PrintOptions2.Default,
            Colors? colors = null);
        void SwitchMode(ColorMode mode);
    }

    public interface IPowerConsolePrinter2 : IColorPrinter2
    {
    }
}