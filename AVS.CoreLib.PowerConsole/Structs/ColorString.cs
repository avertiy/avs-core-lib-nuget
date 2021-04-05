using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Structs
{
    public struct ColorString
    {
        public string Text { get; set; }
        public ColorScheme Color { get; set; }

        public ColorString(string text)
        {
            Text = text;
            Color = ColorScheme.Current;
        }

        public ColorString(string text, ColorScheme color)
        {
            Text = text;
            Color = color;
        }

        public override string ToString()
        {
            return $"$${Text}:-{Color.Foreground} --{Color.Background}$";
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