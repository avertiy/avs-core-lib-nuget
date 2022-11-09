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

        public Cell this[int i] => Cells[i];

        public Row AddCell(Cell cell)
        {
            var colspan = Cells.Sum(x => x.Colspan);
            var cols = colspan + cell.Colspan;
            if (cols > Table.Columns.Count)
                throw new ArgumentOutOfRangeException($"Unable to add another cell. Row colspan #{cols} will exceed the number of columns #{Table.Columns.Count}");

            cell.ColorScheme ??= ColorScheme;
            cell.Column = GetColumn(colspan > 0 ? colspan : 0);
            cell.Row = this;
            Cells.Add(cell);
            return this;
        }

        public override string ToString()
        {
            return string.Join(" | ", Cells);
        }

        private Column GetColumn(int index)
        {
            return Table.Columns[index];
        }
    }


}