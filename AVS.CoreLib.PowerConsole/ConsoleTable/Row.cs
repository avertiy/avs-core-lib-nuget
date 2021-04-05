using System.Collections.Generic;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public class Row
    {
        public Table Table { get; set; }
        public IList<Cell> Cells { get; set; } = new List<Cell>();
        public ColorScheme? ColorScheme { get; set; }
        public void AddCell(object value)
        {
            var cell = Cell.Create(value, null, this);
            Cells.Add(cell);
        }

        public void AddCell(string text, Column column, ColorScheme? scheme = null)
        {
            var cell = Cell.Create(text, column, this, scheme);
            Cells.Add(cell);
        }

        public Cell this[int i] => Cells[i];

        public override string ToString()
        {
            return string.Join(" | ", Cells);
        }
    }
}