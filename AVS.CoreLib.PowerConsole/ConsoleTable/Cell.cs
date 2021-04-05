using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public class Cell
    {
        public string Text { get; set; }
        public int Width { get; set; }

        public bool IsEmpty => string.IsNullOrEmpty(Text);

        public ColorScheme? ColorScheme { get; set; }
        public Column Column { get; set; }
        public Row Row { get; set; }

        public static Cell Create(string text, Column column, Row row, ColorScheme? scheme = null)
        {
            return new Cell(){Text = text, Column = column, Row = row, ColorScheme = scheme};
        }

        public static Cell Create<T>(T obj, Column column, Row row, ColorScheme? scheme = null)
        {
            return new Cell<T>() { Value = obj, Column = column, Row = row, ColorScheme = scheme };
        }

        public override string ToString()
        {
            var text = Text;
            if (Text.Length > Width)
                text = Text.Truncate(Width - 2) + "..";
            else
            {
                text = text.PadRight(Width, ' ');
            }
            return text;
        }

        public ColorString ToColorString()
        {
            var text = Text;
            var spacing = Row?.Table?.Style?.Spacing ?? " ";
            if (Text.Length > Width)
                text = spacing + Text.Truncate(Width - 2 - spacing.Length) + "..";
            else
            {
                text = spacing + text.PadRight(Width - spacing.Length, ' ');
            }

            var scheme = ColorScheme ?? Row?.ColorScheme ?? Column.ColorScheme;
            return scheme.HasValue ? new ColorString(text, scheme.Value) : new ColorString(text);
        }
    }

    public class Cell<T> : Cell
    {
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                Text = value.ToString();
            }
        }
    }

}