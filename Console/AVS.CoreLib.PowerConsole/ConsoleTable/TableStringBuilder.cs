using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public class TableStringBuilder
    {
        private readonly StringBuilder sb = new StringBuilder();
        public string BorderLine { get; set; } = null!;
        public TableStyle Style { get; set; } = null!;

        public void AddTitle(string? title)
        {
            if (string.IsNullOrEmpty(title))
                return;
            sb.AppendLine(BorderLine);
            sb.Append(Style.Bar);
            var pad = (BorderLine.Length - title.Length) / 2;
            sb.Pad(' ', pad);
            sb.Append(title);
            sb.Pad(' ', BorderLine.Length - pad - title.Length - 2);
            sb.Append(Style.Bar);
            sb.AppendLine();
        }

        public void AddColumns(IList<Column>? columns)
        {
            if (columns == null || columns.Count == 0)
                return;
            sb.AppendLine(BorderLine);

            for (var i = 0; i < columns.Count; i++)
            {
                if (i == 0)
                    sb.Append(Style.Bar);

                var column = columns[i];
                sb.Append(column.ToString());
                sb.Append(Style.Bar);
            }

            sb.AppendLine();
        }

        public void AddRows(IList<Row>? rows)
        {
            if (rows == null || rows.Count == 0)
                return;

            sb.AppendLine(BorderLine);

            var colCount = rows.Max(x => x.Cells.Count);
            var fullRow = rows.First(x => x.Cells.Count == colCount);

            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var row = rows[rowIndex];
                if (row.Height <= 1)
                {
                    AppendInLineRow(row, colCount, fullRow);
                }
                else
                {
                    AppendMultiLineRow(row, colCount, fullRow);
                }
            }

            sb.AppendLine(BorderLine);
        }

        private void AppendMultiLineRow(Row row, int cols, Row fullRow)
        {
            for (var l = 0; l < row.Height; l++)
            {
                sb.Append(Style.Bar);
                for (var i = 0; i < cols; i++)
                {
                    var width = fullRow[i].Width;
                    AppendCell(row, i, width, l);
                    sb.Append(Style.Bar);
                }
                sb.AppendLine();
            }

        }

        private void AppendCell(Row row, int index, int width, int line)
        {
            if (row.Cells.Count <= index || line >= row.Cells[index].Height)
            {
                sb.Pad(' ', width);
                return;
            }

            var cell = row.Cells[index];

            //if(line > 0)

            var spacing = Style.Spacing ?? " ";

            var text = cell.Text ?? "";

            if (cell.Height > 1)
            {
                var parts = text.Split(Environment.NewLine);
                if (line < parts.Length)
                {
                    text = spacing + parts[line].Trim().PadRight(width - spacing.Length, ' ');
                }
                else
                {
                    sb.Pad(' ', width);
                    return;
                }
            }
            else if (text.Length <= width)
            {
                text = spacing + text.Replace(Environment.NewLine, ";").PadRight(width - spacing.Length, ' ');
            }
            else
            {
                text = spacing + text.Truncate(width - 2 - spacing.Length) + "..";
            }

            sb.Append(text);
        }

        private void AppendInLineRow(Row row, int cols, Row fullRow)
        {
            sb.Append(Style.Bar);
            for (var i = 0; i < cols; i++)
            {
                var width = fullRow[i].Width;
                AppendCell(row, i, width, 0);
                sb.Append(Style.Bar);
            }
            sb.AppendLine();
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}