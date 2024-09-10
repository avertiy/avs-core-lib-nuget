using System;
using System.Threading;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Extensions.AutoFormatters;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole
{
    using Console = System.Console;

    //todo BIG REWORK
    // 1. create PowerConsole2 with only basic print functionality based on PrintOptions2 enum
    // 2. PrintF methods are rarely used so no need to maintain that stuff.
    // 3. i don't want to put colors for every print method i'd like to printer auto-highlight args
    // for this make all complex print methods like print table accept FString or FormattableString2
    // that carries string format and args so printer will utilize some ArgsFormatProvider
    // to format/colorize arguments rather than track color tags etc.

    // 4. Printer should delegate formatting object & coloring it to some  AutoFormatProvider.Format(key, obj)

    //PowerConsole.Print base features:
    //PowerConsole.Print("{arg:Green C} some text {arg2:Red N3}"); // explicit format modifiers and colors
    //PowerConsole.Print("{arg:C}"); //similar to LogInformation("{arg:C}", 1.022); => <Green>$1.02</Green> - colors is determined based on formatted value (currency value -> green)
    //PowerConsole.Print("<Blue>{arg1:-Green C} blue text</Blue> {arg2:Red N3}");
    //PowerConsole.Print("{arg1:C} some text {arg2:N3}", ColorPalette.RedGreen);//arg1 -> red, arg2 -> green
    //PowerConsole.Print("{arg1:C} some text {timestamp}");//auto-format & color based on argument type: arg1 -> cash -> DarkGreen, arg2 -> DateTime -> Cyan

    // print complex structures
    // (format and color with AutoFormatProvider)
    //PowerConsole.Printer.PrintArray(..)  also it might have a shortcut(s): PowerConsole.PrintArray(..) all these methods could be extensions
    //PowerConsole.Printer.PrintDictionary(..)
    //PowerConsole.Printer.PrintJson(obj)
    //PowerConsole.Printer.PrintList(..)
    //PowerConsole.Printer.PrintObject(obj) //reflect object props & its values, than auto-format & auto-color values with AutoFormatProvider
    //PowerConsole.Printer.PrintTable(obj);
    //PowerConsole.Printer.PrintTable(new [] { obj1, obj2,...})

    //todo print time labels implement via printer and datetimehelper system time instead of DateTime.Now
    //add overload print methods to print elapsed time

    public static partial class PowerConsole
    {
        static PowerConsole()
        {
            ColorScheme.Default = new ColorScheme(Console.ForegroundColor, Console.BackgroundColor);
            DefaultSchemeBackup = ColorScheme.Default;
            AutoFormatter.Instance.AddOptionsFormatter();
        }

        private static ColorScheme DefaultSchemeBackup { get; set; }
        public static ColorScheme CurrentColorScheme => ColorScheme.GetCurrentScheme();
        public static ColorScheme PreviousScheme = new ColorScheme(Console.ForegroundColor, Console.BackgroundColor);

        public static void ApplyColorScheme(ColorScheme scheme)
        {
            Console.ForegroundColor = scheme.Foreground;
            Console.BackgroundColor = scheme.Background;
        }

        public static void ApplyColors(Colors colors)
        {
            if (colors.Foreground.HasValue)
                Console.ForegroundColor = colors.Foreground.Value;

            if (colors.Background.HasValue)
                Console.BackgroundColor = colors.Background.Value;
        }

        /// <summary>
        /// Applies default color scheme
        /// </summary>
        public static void ColorSchemeReset()
        {
            ApplyColorScheme(ColorScheme.Default);
        }

        public static void SetDefaultColorScheme(ColorScheme scheme)
        {
            ColorScheme.Default = scheme;
        }

        /// <summary>
        /// Restores ColorScheme.Default to ColorScheme.Classic
        /// </summary>
        public static void RestoreDefaultColorScheme()
        {
            ColorScheme.Default = DefaultSchemeBackup;
        }

        public static void ClearLine(int left = 0)
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(left, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth - left));
            Console.SetCursorPosition(left, currentLineCursor);
        }

        public static void ClearRegion(int left, int top, int rows)
        {
            var clearLine = new string(' ', Console.WindowWidth - left);
            for (var i = 0; i < rows; i++)
            {
                Console.SetCursorPosition(left, top + i);
                Console.Write(clearLine);
            }
            Console.SetCursorPosition(left, top);
        }

        public static void PressEnterToExit()
        {
            Console.Write("Press enter to quit.");
            var input = Console.ReadKey();
            while (input.Key != ConsoleKey.Enter)
            {
                input = Console.ReadKey();
                Thread.Sleep(100);
            }
        }

        public static void PressQToExit()
        {
            Console.Write("Press `Q` to quit.");
            var input = Console.ReadKey();
            while (input.Key != ConsoleKey.Q)
            {
                input = Console.ReadKey();
                Thread.Sleep(100);
            }
        }
    }
}
