﻿using System;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.Text.TextProcessors;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public interface IPrinter
    {
        string TimeFormat { get; set; }
        void Write(string str, bool endLine = false);
        void Write(string str, ConsoleColor color, bool endLine = false);
        void Write(string str, PrintOptions options);

        void WriteLine(bool voidMultipleEmptyLines = true);
        void WriteLine(string? str, PrintOptions options);
        void Print(string message, PrintOptions options);

        void Print(FormattableString str, PrintOptions options);

        void Print(string message, PrintOptions2 options);

        void SwitchMode(ColorMode mode);
    }

    /// <remarks>
    /// strings staring with @ symbol are treated as strings with expression(s) and processed by TextProcessor(s) this is part of X.Format(); 
    /// <see cref="TextExpressionProcessor"/>
    /// </remarks>
    public interface IXPrinter : IPrinter
    {
        void PrintF(FormattableString str, PrintOptions options);
        //void SetCustomFormatter(Func<FormattableString, string> formatter, bool printF = true);
    }
}