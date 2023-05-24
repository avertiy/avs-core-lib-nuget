using System;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text;
using AVS.CoreLib.Extensions;
namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public static class RowExtensions
    {
        public static Row AddXCell(this Row row, FormattableString str, int colspan = 1, ColorScheme? scheme = null)
        {
            var text = X.Format(str);
            var cell = new Cell() { Text = text, ColorScheme = scheme, Colspan = colspan };
            row.AddCell(cell);
            return row;
        }

        public static Row AddCell(this Row row, string text, int colspan = 1, ColorScheme? scheme = null)
        {
            var (width, height) = text.GetWidthAndHeight();
            var cell = new Cell()
            {
                Text = text,
                ColorScheme = scheme,
                Colspan = colspan,
                Width = width + 2,
                Height = height
            };
            return row.AddCell(cell);
        }

        public static void AddCells(this Row row, string[] values, ColorScheme[]? cellSchemes = null)
        {
            for (var i = 0; i < values.Length && i < row.Table.Columns.Count; i++)
            {
                ColorScheme? cellScheme = null;
                if (cellSchemes != null)
                    cellScheme = cellSchemes[i];

                row.AddCell(values[i], 1, cellScheme);
            }
        }
    }
}