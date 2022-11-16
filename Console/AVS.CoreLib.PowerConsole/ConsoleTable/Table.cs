using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public class Table
    {
        private const int MAX_WIDTH = 160;
        public IList<Column> Columns { get; set; } = new List<Column>();
        public IList<Row> Rows { get; set; } = new List<Row>();
        public int TotalWidth { get; private set; }
        public bool AutoWidth { get; set; } = true;

        public TableStyle Style { get; set; } = new TableStyle();

        public void CalculateWidth(bool force = false)
        {
            if (TotalWidth > 0 && !force)
                return;

            TotalWidth = 0;
            foreach (var column in Columns)
            {
                var minWidth = column.Title.Length + 2;
                if (column.Width < minWidth)
                    column.Width = minWidth;
                TotalWidth += column.Width;
            }

            if (TotalWidth > MAX_WIDTH)
                throw new TableException($"Table total width {TotalWidth} exceeds MAX_WIDTH {MAX_WIDTH}");

            for (var i = 0; i < Columns.Count; i++)
            {
                var column = Columns[i];
                foreach (var row in Rows)
                {
                    if (i >= row.Cells.Count)
                        continue;

                    var cell = row[i];
                    if (cell.Column == null)
                    {
                        cell.Column = column;
                    }

                    if (string.IsNullOrEmpty(cell.Text))
                        continue;

                    if (cell.Text.Length > column.Width)
                    {
                        var diff = cell.Text.Length - column.Width;
                        if (TotalWidth + diff + 2 < MAX_WIDTH)
                        {
                            column.Width = cell.Text.Length + 2;
                            TotalWidth += diff + 2;
                        }
                        else if (TotalWidth + diff < MAX_WIDTH)
                        {
                            column.Width = cell.Text.Length;
                            TotalWidth += diff;
                        }
                    }
                }
            }

            TotalWidth += Columns.Count * 2;

            foreach (var row in Rows)
            {
                var index = 0;
                foreach (var cell in row.Cells)
                {
                    if (cell.Colspan == 1)
                    {
                        cell.Width = cell.Column.Width;
                        index++;
                        continue;
                    }

                    for (var i = 0; i < cell.Colspan; i++)
                    {
                        var ind = index + i;
                        if (ind >= Columns.Count)
                            break;

                        cell.Width += Columns[ind].Width;
                    }
                    cell.Width += cell.Colspan - 1;
                    index += cell.Colspan;
                }
            }

            //for (var i = 0; i < Columns.Count; i++)
            //{
            //    var column = Columns[i];
            //    foreach (var row in Rows)
            //    {
            //        if (i >= row.Cells.Count)
            //            continue;

            //        var cell = row[i];
            //        cell.Width = column.Width;
            //    }
            //}

        }

        public override string ToString()
        {
            if (AutoWidth)
                CalculateWidth();
            var sb = new StringBuilder();

            var line = this.GetBorderLine();
            sb.AppendLine(line);

            for (var i = 0; i < Columns.Count; i++)
            {
                if (i == 0)
                    sb.Append(Style.Bar);

                var column = Columns[i];
                sb.Append(column.ToString());
                sb.Append(Style.Bar);
            }
            sb.AppendLine();
            sb.AppendLine(line);

            foreach (var row in Rows)
            {
                for (var i = 0; i < row.Cells.Count; i++)
                {
                    if (i == 0)
                        sb.Append(Style.Bar);

                    var cell = row.Cells[i];
                    sb.Append(cell.ToString());
                    sb.Append(Style.Bar);
                }
                sb.AppendLine();
            }

            sb.AppendLine(line);
            return sb.ToString();
        }

        public string GetBorderLine()
        {
            var style = Style;
            return style.Cross + string.Join(style.Cross, Columns.Select(x => "".PadRight(x.Width, style.Pad))) + style.Cross;
        }

        public void AddColumn(string title, ColorScheme? scheme = null, int? width = null)
        {
            var minWidth = title.Length + 2;
            var colWidth = minWidth;

            if (width.HasValue && width > minWidth)
                colWidth = width.Value;

            Columns.Add(new Column() { Title = title, Width = colWidth, ColorScheme = scheme });
        }

        public Row AddRow(ColorScheme? scheme = null)
        {
            var row = new Row() { ColorScheme = scheme, Table = this };
            Rows.Add(row);
            return row;
        }

        public void AddRow(object[] values, ColorScheme? scheme = null)
        {
            var row = new Row() { ColorScheme = scheme, Table = this };
            for (var i = 0; i < values.Length && i < Columns.Count; i++)
            {
                row.AddCellWithValue(values[i]);
            }
            Rows.Add(row);
        }

        public static Table Create<T>(IEnumerable<T> data)
        {
            var type = typeof(T);
            var allProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var props = allProperties.Where(p => p.CanRead).ToArray();
            if (props.Length == 0)
                throw new ArgumentException($"Type {type.Name} does not have public instance properties unable to build columns");

            var table = new Table { Columns = props.Select(pi => new Column() { Title = pi.Name }).ToList() };

            foreach (var obj in data)
            {
                table.AddRow(obj, props);
            }

            table.CalculateWidth();
            return table;
        }

        public static Table Create(string[] columns, ColorScheme[] schemes = null)
        {
            var table = new Table();
            for (var i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                ColorScheme? scheme = null;
                if (schemes != null && schemes.Length > i)
                    scheme = schemes[i];

                table.AddColumn(column, scheme);
            }

            return table;
        }
    }
}