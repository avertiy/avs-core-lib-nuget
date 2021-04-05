using System.Linq;
using System.Reflection;
using System.Text;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public static class TableExtensions
    {
        public static string GetBorderLine(this Table tbl)
        {
            var style = tbl.Style;
            return style.Cross + string.Join(style.Cross, tbl.Columns.Select(x => "".PadRight(x.Width, style.Pad))) + style.Cross;
        }

        internal static void AddRow<T>(this Table table, T obj, PropertyInfo[] props)
        {
            var row = new Row() { Table = table };

            foreach (var pi in props)
            {
                var value = pi.GetValue(obj);
                row.AddCell(value);
            }
            table.Rows.Add(row);
        }

        public static ColorFormattedString ToColorFormattedString(this Table table, bool useAutoWidth = true)
        {
            if (useAutoWidth)
                table.CalculateWidth();
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
            return new ColorFormattedString(sb.ToString());
        }
    }
}