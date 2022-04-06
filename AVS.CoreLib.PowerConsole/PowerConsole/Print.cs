using System;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole
{

    /// <summary>
    /// PowerConsole represents simple extensions over standard .NET Console functionality
    /// If you need more rich & extensive console frameworks check out links below  
    /// </summary>
    /// <seealso>https://github.com/Athari/CsConsoleFormat - advanced formatting of console output for .NET</seealso>
    /// <seealso>https://github.com/migueldeicaza/gui.cs - Terminal GUI toolkit for .NET</seealso>
    /// <seealso>http://elw00d.github.io/consoleframework/- cross-platform toolkit that allows to develop TUI applications using C# and based on WPF-like concepts</seealso>
    public static partial class PowerConsole
    {
        public static void Print(string str)
        {
            Write(str);
            WriteEndLine(true);
        }

        public static void Print(string str, bool endLine)
        {
            Write(str);
            WriteEndLine(endLine);
        }

        public static void Print(string str, ConsoleColor color, bool endLine = true)
        {
            Write(str, color);
            WriteEndLine(endLine);
        }

        public static void Print(string str, ColorScheme scheme, bool endLine = true)
        {
            ApplyColorScheme(scheme);
            Write(str);
            WriteEndLine(endLine);
            ColorSchemeReset();
        }
    }
}
