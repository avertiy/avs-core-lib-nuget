using System;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public class ColorPalette
    {
        public static ColorPalette RedGreen => new ColorPalette(ConsoleColor.DarkGreen, ConsoleColor.DarkRed);
        public static ColorPalette BlueGreen => new ColorPalette(ConsoleColor.DarkGreen, ConsoleColor.Blue);

        public ConsoleColor[] Colors { get; set; }

        public ConsoleColor this[int i] => Colors[i];

        public ColorPalette(params ConsoleColor[] colors)
        {
            Colors = colors;
        }

        public static implicit operator ColorPalette(ConsoleColor[] colors)
        {
            return new ColorPalette(colors);
        }
    }

    
}