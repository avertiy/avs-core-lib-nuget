using System;
using AVS.CoreLib.Extensions.Stringify;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text;

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

        public static Row AddCellWithValue(this Row row, string value, int colspan = 1, ColorScheme? scheme = null)
        {
            var cell = new Cell()
            {
                Text = value,
                ColorScheme = scheme,
                Colspan = colspan
            };
            return row.AddCell(cell);
        }

        public static Row AddCellWithValue<T>(this Row row, T value, int colspan = 1, ColorScheme? scheme = null)
        {
            var cell = new Cell()
            {
                Value = value, 
                Text = value?.Stringify(),
                ColorScheme = scheme,
                Colspan = colspan
            };
            return row.AddCell(cell);
        }

        public static Row AddCell(this Row row, string text, int colspan = 1, ColorScheme? scheme = null)
        {
            var cell = new Cell() { Text = text, ColorScheme = scheme, Colspan = colspan };
            return row.AddCell(cell);
        }

        public static void AddCells(this Row row, string[] values, ColorScheme[] cellSchemes = null)
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