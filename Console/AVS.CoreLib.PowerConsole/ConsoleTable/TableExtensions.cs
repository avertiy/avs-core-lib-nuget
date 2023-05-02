using System.Linq;
using System.Reflection;
using System.Text;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public static class TableExtensions
    {
        internal static Row AddRow<T>(this Table table, T obj, PropertyInfo[] props)
        {
            var row = new Row() { Table = table };

            foreach (var pi in props)
            {
                var value = pi.GetValue(obj);
                row.AddCellWithValue(value);
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
            var scheme = cell.ColorScheme ?? row?.ColorScheme ?? cell.Column.ColorScheme;
            return scheme.HasValue ? new ColorString(text, scheme.Value) : new ColorString(text);
        }
    }
}