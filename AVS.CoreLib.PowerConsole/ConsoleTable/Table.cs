using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public class Table
    {
        private const int MAX_WIDTH = 160;
        public IList<Column> Columns { get; set; } = new List<Column>();
        public IList<Row> Rows { get; set; } = new List<Row>();
        public int TotalWidth { get; private set; }

        public TableStyle Style { get; set; } = new TableStyle();

        public void CalculateWidth(bool force = false)
        {
            if(TotalWidth > 0 && !force)
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
                throw new ConsoleException($"Table total width {TotalWidth} exceeds MAX_WIDTH {MAX_WIDTH}");

            for (var i = 0; i < Columns.Count; i++)
            {
                var column = Columns[i];
                foreach (var row in Rows)
                {
                    var cell = row[i];
                    cell.Column = column;
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

            TotalWidth += Columns.Count*2;
            for (var i = 0; i < Columns.Count; i++)
            {
                var column = Columns[i];
                foreach (var row in Rows)
                {
                    var cell = row[i];
                    cell.Width = column.Width;
                }
            }

        }

        public string ToString(bool useAutoWidth = true)
        {
            if (useAutoWidth)
                CalculateWidth();
            return $"{string.Join(" | ", Columns)}\r\n{string.Join("\r\n", Rows)}";
        }

        public void AddColumn(string title, ColorScheme? scheme = null, int? width = null)
        {
            var minWidth = title.Length + 2;
            var colWidth = minWidth;

            if(width.HasValue && width > minWidth)
                colWidth = width.Value;

            Columns.Add(new Column() { Title = title, Width = colWidth, ColorScheme = scheme});
        }

        public void AddRow(object[] values, ColorScheme? scheme = null)
        {
            var row = new Row(){ ColorScheme = scheme, Table = this};
            for (var i = 0; i < values.Length && i < Columns.Count; i++)
            {
                row.AddCell(values[i]);
            }
            Rows.Add(row);
        }

        public void AddRow(string[] values, ColorScheme? scheme = null, ColorScheme[] cellSchemes = null)
        {
            var row = new Row() { ColorScheme = scheme, Table = this};
            for (var i = 0; i < values.Length && i < Columns.Count; i++)
            {
                ColorScheme? cellScheme = null;
                if (cellSchemes != null)
                    cellScheme = cellSchemes[i];

                row.AddCell(values[i], Columns[i], cellScheme);
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

            var table = new Table {Columns = props.Select(pi => new Column() {Title = pi.Name}).ToList()};
            
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