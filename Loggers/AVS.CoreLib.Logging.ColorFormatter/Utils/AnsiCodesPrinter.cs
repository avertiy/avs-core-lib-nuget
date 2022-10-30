namespace AVS.CoreLib.Logging.ColorFormatter.Utils;

public static class AnsiCodesPrinter
{
    public static void PrintAllCodes()
    {
        //decorations
        Console.WriteLine("Decorations:");
        PrintCodes(0, 1, 2, 3, 4, 7);

        // foreground colors
        Console.WriteLine("Foreground colors:");
        PrintCodes(30, 38);
        PrintCodes(90, 98);

        // bg colors
        Console.WriteLine("Background colors:");
        PrintCodes(40, 48);
        PrintCodes(100, 108);
        Console.WriteLine(AnsiCodes.RESET);
    }
    public static void PrintCodes(params int[] codes)
    {
        foreach (var i in codes)
        {
            var codeStr = AnsiCodes.Code(i);
            var brightStr = AnsiCodes.Bright(i);
            var esc = codeStr.Esc();
            var esc2 = brightStr.Esc();
            var text = $"({i}) {codeStr} code: {esc}{AnsiCodes.RESET} | {brightStr} bright: {esc2}{AnsiCodes.RESET};";
            Console.WriteLine(text);
        }
        Console.WriteLine(AnsiCodes.RESET);
    }
    public static void PrintCodes(int from, int to)
    {
        PrintCodes(Enumerable.Range(from, to - from).ToArray());
    }

    public static void PrintRgbColors(int redStep = 40, int greenStep = 40, int blueStep = 40, int filterDarkColors = 280)
    {
        foreach (var (r, g, b) in GetRgbColors(redStep, greenStep, blueStep).Where(x => x.R + x.B + x.G > filterDarkColors))
        {
            var rgb = $"{r},{g},{b}";
            var code = AnsiCodes.Rgb(r, g, b);
            var esc = code.Esc();
            var text = $"{code}{esc}{AnsiCodes.RESET}";
            Console.WriteLine(text);
        }
        Console.WriteLine(AnsiCodes.RESET);
    }



    //public static void PrintPalete(bool foregroundColors = true, bool backgroundColors = true)
    //{
    //    Console.WriteLine("Ansi color palete");
    //    var colors = Enum.GetValues<ConsoleColor>();

    //    if (foregroundColors)
    //    {
    //        Console.WriteLine("Foreground colors:\r\n");
    //        foreach (var color in colors)
    //        {
    //            var foreground = GetForegroundColorEscapeCode(color);
    //            var suffix = foreground == DEFAULT_FOREGROUND_COLOR ? "[DEFAULT_FOREGROUND_COLOR]" : "";

    //            Console.Write("Color: ");
    //            Write(color.ToString(), color);
    //            Console.Write($" => {foreground.Replace("\x1B", "\\x1B")}");
    //            Console.Write(suffix + $" {foreground}test color" + DEFAULT_FOREGROUND_COLOR);
    //            Console.WriteLine();
    //        }
    //    }

    //    if (backgroundColors)
    //    {
    //        Console.WriteLine("Background colors:\r\n");
    //        foreach (var color in colors)
    //        {
                
    //            var background = GetBackgroundColorEscapeCode(color);
    //            var suffix = background == DEFAULT_BACKGROUND_COLOR ? "[DEFAULT_FOREGROUND_COLOR]" : "";

    //            Console.Write("Color: ");
    //            WriteB(color.ToString(), color);
    //            Console.Write($" => {background.Replace("\x1B", "\\x1B")}");
    //            Console.Write(suffix + $" {background}test color" + DEFAULT_BACKGROUND_COLOR);
    //            Console.WriteLine();
    //        }
    //    }
    //}

    public static IEnumerable<(byte R, byte G, byte B)> GetRgbColors(int redStep, int greenStep, int blueStep)
    {
        for (var r = 0; r <= 255; r++)
        {
            for (var g = 0; g <= 255; g++)
            {
                for (var b = 0; b <= 255; b++)
                {
                    yield return ((byte)r, (byte)g, (byte)b);
                    b += blueStep;
                }
                g += greenStep;
            }
            r += redStep;
        }
    }
}