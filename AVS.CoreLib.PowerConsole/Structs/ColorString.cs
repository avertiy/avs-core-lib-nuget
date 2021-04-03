using System;

namespace AVS.CoreLib.PowerConsole.Structs
{
    public struct ColorString
    {
        public string Text { get; set; }
        public ConsoleColor Color { get; set; }

        public ColorString(string text, ConsoleColor color)
        {
            Text = text;
            Color = color;
        }

    }
}
//    public class ColorText : IColorText
//    {
//        private readonly StringBuilder _stringBuilder = new StringBuilder();
//        public ConsoleColor Color { get; set; }
//        public string Text => _stringBuilder.ToString();

//        public ColorText()
//        {
//        }

//        public ColorText(ConsoleColor color)
//        {
//            Color = color;
//        }

//        public void Append(string text)
//        {
//            _stringBuilder.Append(text);
//        }

//        public static ColorText operator +(ColorText text, string text)
//        {
//            text
//        }
//    }
//}