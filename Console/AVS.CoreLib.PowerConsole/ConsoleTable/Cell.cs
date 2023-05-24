using System;
using System.Linq;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Stringify;
using AVS.CoreLib.PowerConsole.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public class Cell
    {
        public object? Value { get; set; }
        public string? Text { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Colspan { get; set; } = 1;

        public ColorScheme? ColorScheme { get; set; }

        public Row Row { get; set; } = null!;


        public static Cell Create(string text, Column column, Row row, ColorScheme? scheme = null)
        {
            var (width, height) = text.GetWidthAndHeight();
            return new Cell()
            {
                Text = text, 
                Row = row, 
                ColorScheme = scheme,
                Width = width+2,
                Height = height,
            };
        }

        public override string ToString()
        {
            var row = Row;
            var width = Width;
            if (Colspan > 1)
            {
                var index = row.Cells.IndexOf(this);
                if (index >= 0 && index + Colspan < row.Cells.Count)
                {
                    for (var i = 1; i < Colspan; i++)
                    {
                        width += row[i + index].Width;
                    }
                }
            }

            var text = Text;
            var spacing = row?.Table?.Style?.Spacing ?? " ";
            if (text.Length > width)
                text = spacing + text.Truncate(width - 2 - spacing.Length) + "..";
            else
            {
                text = spacing + text.PadRight(width - spacing.Length, ' ');
            }

            return text;
        }
    }

    public static class CellExtensions
    {
        internal static (int width, int height) GetWidthAndHeight(this string str)
        {
            var arr = str.Split(Environment.NewLine);
            var height = arr.Length;
            var width = arr.Max(x => x.Length);
            return (width, height);
        }

        internal static int GetHeight(this string str)
        {
            return str.Count(Environment.NewLine) + 1;
        }

        public static int CalcWidth(this Cell cell, int index, int columnWidth)
        {
            var width = cell.Width;
            width = width > 0 ? width : cell.Text?.Length + 2 ?? 0;
            var colspan = cell.Colspan;

            if (colspan == 1)
            {
                width = columnWidth >= width ? columnWidth : width;
            }
            else
            {
                var columns = cell.Row.Table.Columns;
                for (var i = 0; i < colspan; i++)
                {
                    var ind = index + i;
                    if (ind >= columns.Count)
                        break;

                    width += columns[ind].Width;
                }

                width += colspan - 1;
            }
            
            cell.Width = width;
            return width;
        }

        public static bool IsEmpty(this Cell cell)
        {
            return string.IsNullOrEmpty(cell.Text);
        }


    }
}