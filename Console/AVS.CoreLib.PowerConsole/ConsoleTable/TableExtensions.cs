using System.Linq;
using System.Reflection;
using System.Text;
using AVS.CoreLib.Extensions.Stringify;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public static class TableExtensions
    {
        public static Table WithTitle(this Table table, string title)
        {
            if(!string.IsNullOrEmpty(title))
                table.Title = title;
            return table;
        }

        internal static string GetBorderLine(this Table table)
        {
            var style = table.Style;

            var pad = "";
            if (table.Columns.Any())
            {
                pad = string.Join(style.Cross, table.Columns.Select(x => "".PadRight(x.Width, style.Pad)));
            }
            else if (table.Rows.Any())
            {
                pad = string.Join(style.Cross, table.Rows[0].Cells.Select(x => "".PadRight(x.Width, style.Pad)));
            }

            return style.Cross + pad + style.Cross;
        }

        public static void AddColumns(this Table table, params string[] columns)
        {
            foreach (var column in columns)
                table.AddColumn(column);
        }

        public static void AddRow(this Table table, object[] values, ColorScheme? scheme = null)
        {
            var row = table.AddRow(new Row() { ColorScheme = scheme });
            for (var i = 0; i < values.Length; i++)//&& i < table.Columns.Count
            {
                var text = values[i].Stringify();
                row.AddCell(text);
            }
        }

        public static Row AddRow(this Table table, ColorScheme? scheme = null)
        {
            return table.AddRow(new Row() { ColorScheme = scheme });
        }

        internal static Row AddRow<T>(this Table table, T obj, PropertyInfo[] props)
        {
            var row = new Row() { Table = table };

            foreach (var pi in props)
            {
                var value = pi.GetValue(obj);
                var str = value.Stringify(pi.Name);
                row.AddCell(str);
            }
            table.Rows.Add(row);
            return row;
        }

        public static ColorMarkupString ToColorFormattedString(this Table table, bool useAutoWidth = true)
        {
            if (useAutoWidth)
                table.CalculateWidth(force: true);
            var sb = new StringBuilder();

            var line = table.GetBorderLine();
            sb.AppendLine(line);
            for (var i = 0; i < table.Columns.Count; i++)
            {
                if (i == 0)
                    sb.Append(table.Style.Bar);
                var column = table.Columns[i];
                sb.Append(column.ToColorString());
                sb.Append(table.Style.Bar);
            }
            sb.AppendLine();
            sb.AppendLine(line);

            foreach (var row in table.Rows)
            {
                for (var i = 0; i < row.Cells.Count; i++)
                {
                    if (i == 0)
                        sb.Append(table.Style.Bar);

                    var cell = row.Cells[i];
                    sb.Append(cell.ToColorString());
                    sb.Append(table.Style.Bar);
                }
                sb.AppendLine();
            }
            sb.AppendLine(line);
            return new ColorMarkupString(sb.ToString());
        }

        public static ColorString ToColorString(this Cell cell)
        {
            var row = cell.Row;
            var text = cell.ToString();
            var scheme = cell.ColorScheme ?? row?.ColorScheme;
            return scheme.HasValue ? new ColorString(text, scheme.Value) : new ColorString(text);
        }

        public static void CalculateWidth(this Table table, bool force = false)
        {
            //if (TotalWidth > 0 && force == false)
            //    return;

            var totalWidth = 0;
            var columns = table.Columns;
            var rows = table.Rows;
            foreach (var column in table.Columns)
            {
                var minWidth = column.Title.Length + 2;
                if (column.Width < minWidth)
                    column.Width = minWidth;
                totalWidth += column.Width;
            }

            if (columns.Any())
            {
                for (var i = 0; i < columns.Count; i++)
                {
                    var column = columns[i];
                    foreach (var row in rows)
                    {
                        if (i >= row.Cells.Count)
                            continue;

                        var cell = row[i];

                        if (string.IsNullOrEmpty(cell.Text))
                            continue;

                        if (cell.Text.Length + 1 > column.Width)
                        {
                            var diff = cell.Text.Length - column.Width;
                            if (totalWidth + diff + 2 < Table.MAX_WIDTH)
                            {
                                column.Width = cell.Text.Length + 2;
                                totalWidth += diff + 2;
                            }
                            else if (totalWidth + diff < Table.MAX_WIDTH)
                            {
                                column.Width = cell.Text.Length;
                                totalWidth += diff;
                            }
                        }
                    }
                }

                totalWidth += columns.Count * 2;

                foreach (var row in rows)
                {
                    var index = 0;
                    for (var i = 0; i < row.Cells.Count; i++)
                    {
                        var cell = row.Cells[i];
                        cell.CalcWidth(index, columns[i].Width);
                        index += cell.Colspan;
                    }
                }
            }
            else if (rows.Any())
            {
                totalWidth = 0;
                var cells = rows.First().Cells;
                for (var i = 0; i < cells.Count; i++)
                {
                    var width = 0;
                    foreach (var row in rows)
                    {
                        if (i >= row.Cells.Count)
                            continue;
                        width = row[i].CalcWidth(0, width);
                    }

                    //ensure all rows has max column width
                    foreach (var row in rows)
                    {
                        if (i >= row.Cells.Count)
                            continue;

                        if (row[i].Width < width)
                            row[i].Width = width;
                    }

                    totalWidth += width + 2;
                }
            }
        }
    }
}