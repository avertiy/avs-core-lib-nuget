using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public class Row
    {
        public Table Table { get; set; }
        public IList<Cell> Cells { get; set; } = new List<Cell>();
        public ColorScheme? ColorScheme { get; set; }
        /// <summary>
        /// Similar to column Width indicates how many lines row is needed
        /// </summary>
        public int Height { get; set; }

        public Cell this[int i] => Cells[i];

        public Row AddCell(Cell cell)
        {
            //var colspan = Cells.Sum(x => x.Colspan);
            cell.ColorScheme ??= ColorScheme;
            //cell.Column = GetColumn(colspan > 0 ? colspan : 0);
            cell.Row = this;
            
            if (Height < cell.Height)
                Height = cell.Height;

            Cells.Add(cell);
            return this;
        }

        public override string ToString()
        {
            return string.Join(" | ", Cells);
        }

        private Column? GetColumn(int index)
        {
            return Table.Columns.Any() ? Table.Columns[index] : null;
        }

        public static Row Create(Table table, params string[] values)
        {
            var row = new Row() { Table = table };
            foreach (var value in values)
            {
                row.AddCell(value);
            }
            return row;
        }
    }


}