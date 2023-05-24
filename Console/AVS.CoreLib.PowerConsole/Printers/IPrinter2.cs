using System;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Enums;

namespace AVS.CoreLib.PowerConsole.Printers
{
    //work in process..
    //IPrinter2 will replace IPrinter
    /// <summary>
    /// Define printer base functionality
    /// </summary>
    public interface IPrinter2
    {
        void Write(string message);
        void WriteLine(string message);
        void Print(string message, PrintOptions2 options);
        void Print(FormattableString str, PrintOptions2 options);
    }

    /// <summary>
    /// Define printer base color functionality
    /// </summary>
    public interface IColorPrinter2
    {
        ColorMode Mode { get; }
        void Write(string message, Colors colors);
        void WriteLine(string message, Colors colors);
        void Print(string message, PrintOptions2 options, Colors colors);
        void Print(FormattableString str, PrintOptions2 options, Colors colors);
        void SwitchMode(ColorMode mode);
    }

    public class PrinterOptions
    {
        public string? TimeFormat { get; set; }
    }

    //public class Color
}