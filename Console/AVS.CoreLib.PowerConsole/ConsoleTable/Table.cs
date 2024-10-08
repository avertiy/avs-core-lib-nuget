﻿using System.Collections.Generic;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public class Table
    {
        public const int MAX_WIDTH = 160;
        public string? Title { get; set; }

        public IList<Column> Columns { get; set; } = new List<Column>();
        public IList<Row> Rows { get; set; } = new List<Row>();

        public TableStyle Style { get; set; } = new TableStyle();

        public override string ToString()
        {
            var builder = new TableStringBuilder()
            {
                BorderLine = this.GetBorderLine(),
                Style = Style,
            };

            builder.AddTitle(Title);
            builder.AddColumns(Columns);
            builder.AddRows(Rows);
            return builder.ToString();
        }

        public int AddColumn(string title, ColorScheme? scheme = null, int? width = null)
        {
            var minWidth = title.Length + 2;
            var colWidth = minWidth;

            if (width.HasValue && width > minWidth)
                colWidth = width.Value;

            Columns.Add(new Column()
            {
                Title = title,
                Width = colWidth,
                ColorScheme = scheme
            });
            return colWidth;
        }

        public Row AddRow(Row row)
        {
            row.Table = this;
            Rows.Add(row);
            return row;
        }

        /// <summary>
        /// Create table from columns 
        /// </summary>
        /// <param name="columns">columns</param>
        /// <param name="schemes">seems not working at the moment</param>
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

        public static Table FromObject(object source, params string[] excludeProperties)
        {
            var builder = new TableBuilder { TableOrientation = TableOrientation.Auto, ExcludeProperties = excludeProperties };
            var table = builder.CreateTable(source);
            return table;
        }

        public static Table FromArray<T>(IEnumerable<T> source, params string[] excludeProperties)
        {
            var builder = new TableBuilder() { TableOrientation = TableOrientation.Auto, ExcludeProperties = excludeProperties };
            var table = builder.CreateTable(source);
            return table;
        }
    }

    public enum TableOrientation
    {
        Auto = 0,
        Vertical = 1,
        Horizontal = 2,
    }
}