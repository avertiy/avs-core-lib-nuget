using System.Collections.Generic;
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
            var totalWidth = 0;
            if (table.Title != null)
            {
                totalWidth = table.Title.Length + 2;
            }
            
            var rows = table.Rows;
            
            if (table.Columns.Any())
            {
                var columns = table.Columns;
                var colsWidth = 0;
                foreach (var column in columns)
                {
                    var minWidth = column.Title.Length + 2;
                    if (column.Width < minWidth)
                        column.Width = minWidth;
                    colsWidth += column.Width;
                }

                var diff = totalWidth - colsWidth;
                if (diff > 0)
                {
                    var addWidth = diff / columns.Count;
                    foreach (var column in columns)
                    {
                        column.Width += addWidth;
                    }
                }

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

                        if (cell.Colspan == 1)
                        {
                            if(cell.Text.Length <= column.Width)
                                continue;
                        
                            column.Width = cell.Text.Length + 2;
                            continue;
                        }

                        //if (cell.Colspan > 1)
                        //{
                        //    continue;
                        //}

                        //if (cell.Text.Length + 1 > column.Width)
                        //{
                        //    var diff = cell.Text.Length - column.Width;
                        //    if (totalWidth + diff + 2 < Table.MAX_WIDTH)
                        //    {
                        //        column.Width = cell.Text.Length + 2;
                        //        totalWidth += diff + 2;
                        //    }
                        //    else if (totalWidth + diff < Table.MAX_WIDTH)
                        //    {
                        //        column.Width = cell.Text.Length;
                        //        totalWidth += diff;
                        //    }
                        //}
                    }
                }

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
                var columns = CalcColumns(rows, totalWidth);
                for (var i = 0; i < columns.Count; i++)
                {
                    foreach (var row in rows)
                    {
                        if (i >= row.Cells.Count)
                            continue;
                        var width = columns[i];
                        if (row[i].Width < width)
                            row[i].Width = width;
                    }
                }
            }
        }

        private static List<int> CalcColumns(IList<Row> rows, int totalWidth)
        {
            var cells = rows.First().Cells;
            var colsWidth = 0;
            var columns = new List<int>(cells.Count + 2);
            for (var i = 0; i < cells.Count; i++)
            {
                var width = 0;
                foreach (var row in rows)
                {
                    if (i >= row.Cells.Count)
                        continue;
                    width = row[i].CalcWidth(0, width);
                }

                columns.Add(width);
                colsWidth += width;
            }

            var diff = totalWidth - colsWidth;
            if (diff > 0 && columns.Any())
            {
                var addWidth = diff / columns.Count;
                for (var i = 0; i < columns.Count; i++)
                {
                    columns[i] += addWidth;
                }
            }

            return columns;
        }
    }
}