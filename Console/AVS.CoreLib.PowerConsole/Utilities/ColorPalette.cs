using System;
using System.Collections;
using System.Collections.Generic;

namespace AVS.CoreLib.PowerConsole.Utilities
{
    /// <summary>
    /// Represent a set of colors
    /// </summary>
    public class ColorPalette : IEnumerable<ConsoleColor>
    {
        public static ColorPalette GrayRedGreen => new ColorPalette(ConsoleColor.DarkGray, ConsoleColor.DarkGreen, ConsoleColor.DarkRed);
        public static ColorPalette RedGreen => new ColorPalette(ConsoleColor.DarkGreen, ConsoleColor.DarkRed);
        public static ColorPalette BlueGreen => new ColorPalette(ConsoleColor.DarkGreen, ConsoleColor.Blue);

        public ConsoleColor[] Colors { get; set; }

        public ConsoleColor this[int i] => Colors[i];

        public ColorPalette(params ConsoleColor[] colors)
        {
            Colors = colors;
        }

        public void Init(params ConsoleColor[] colors)
        {
            Colors = colors;
        }


        public static implicit operator ColorPalette(ConsoleColor[] colors)
        {
            return new ColorPalette(colors);
        }

        public int Length => Colors.Length;

        public IEnumerator<ConsoleColor> GetEnumerator()
        {
            return (IEnumerator<ConsoleColor>)Colors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}