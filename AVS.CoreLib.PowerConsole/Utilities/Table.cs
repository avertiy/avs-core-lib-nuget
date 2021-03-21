using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.PowerConsole.Extensions;

namespace AVS.CoreLib.PowerConsole.Utilities
{
    internal class Table
    {
        private const int MAX_WIDTH = 160;
        public IList<Column> Columns { get; set; } = new List<Column>();
        public IList<Row> Rows { get; set; } = new List<Row>();
        private void SetupColumnsWidth()
        {
            int totalWidth = 0;
            for (var i = 0; i < Columns.Count; i++)
            {
                Columns[0].Width = Columns[i].Title.Length + 2;
                totalWidth += Columns[0].Width;
            }

            if (totalWidth > MAX_WIDTH)
                throw new Exception("too many columns to display in console");

            for (var i = 0; i < Columns.Count; i++)
            {
                var column = Columns[i];
                foreach (var row in Rows)
                {
                    var cell = row[i];
                    if (cell.Value == null)
                        continue;

                    if (cell.Text.Length > column.Width)
                    {
                        var diff = cell.Text.Length - column.Width;
                        if (totalWidth + diff + 2 < MAX_WIDTH)
                        {
                            column.Width = cell.Text.Length + 2;
                            totalWidth += diff + 2;
                        }
                        else if (totalWidth + diff < MAX_WIDTH)
                        {
                            column.Width = cell.Text.Length;
                            totalWidth += diff;
                        }
                    }
                }
            }

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


        public override string ToString()
        {
            return $"{string.Join(" | ", Columns)}\r\n{string.Join("\r\n", Rows)}";
        }

        public static Table Create<T>(IEnumerable<T> data)
        {
            var type = typeof(T);
            var allProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var props = allProperties.Where(p => p.CanRead).ToArray();


            var table = new Table
            {
                Columns = props.Select(pi => new Column() { Title = pi.Name }).ToList(),
                Rows = data.Select(row => Row.Create(row, props)).ToList()
            };
            table.SetupColumnsWidth();
            return table;
        }

        public class Column
        {
            public string Title { get; set; }
            public int Width { get; set; }

            public override string ToString()
            {
                var pad = (Width - Title.Length) / 2 + Title.Length;
                var str = Title.PadLeft(pad, ' ').PadRight(Width, ' ');
                return str;
            }
        }

        public class Row
        {
            public IList<Cell> Cells { get; set; }

            public void AddCell(object value)
            {
                var cell = new Cell(value);
                Cells.Add(cell);
            }

            public Cell this[int i] => Cells[i];

            public override string ToString()
            {
                return string.Join(" | ", Cells);
            }

            public static Row Create<T>(T obj, PropertyInfo[] props)
            {
                var row = new Row() { Cells = new List<Cell>() };

                foreach (var pi in props)
                {
                    var value = pi.GetValue(obj);
                    row.AddCell(value);
                }
                return row;
            }
        }

        public class Cell
        {
            public Cell(object value)
            {
                Value = value;
                Text = value?.ToString();
            }

            public object Value { get; }
            public string Text { get; set; }

            public int Width { get; set; }

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
        }
    }
}