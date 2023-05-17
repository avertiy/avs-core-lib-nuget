using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.Extensions.Stringify;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public class TableBuilder
    {
        public ColumnOptions ColumnOptions { get; set; }
        public string[]? ExcludeProperties { get; set; }

        private bool FilterProperties(string prop, object value)
        {
            var skip = ExcludeProperties != null && ExcludeProperties.Contains(prop) || value.IsEmpty();
            return !skip;
        }

        public Table CreateTable<T>(T source)
        {
            Guard.Against.Null(source, "Source is missing");
            var table = new Table();

            var type = typeof(T);
            var dict = type.Reflect(source!, FilterProperties);

            if (ColumnOptions == ColumnOptions.Auto)
            {
                var width = dict.Keys.Sum(x => x.Length+3);
                if (width < 150)
                    BuildHorizontal(table, dict);
                else
                    BuildVertical(table, type.GetReadableName(), dict);
            }

            if (ColumnOptions == ColumnOptions.Horizontal)
            {
                BuildHorizontal(table, dict);
            }
            else if (ColumnOptions == ColumnOptions.Vertical)
            {
                BuildVertical(table, type.GetReadableName(), dict);
            }

            table.CalculateWidth();
            return table;
        }

        private void BuildVertical(Table table, string typeName, Dictionary<string, object> dict)
        {
            table.AddColumn(typeName);
            table.AddColumn("Values");
            foreach (var kp in dict)
            {
                var str = kp.Value.Stringify(kp.Key);
                table.AddRow(Row.Create(table, kp.Key, str));
            }
        }

        private void BuildHorizontal(Table table, Dictionary<string, object> dict)
        {
            var rowValues = new List<string>();
            foreach (var kp in dict)
            {
                table.AddColumn(kp.Key);
                rowValues.Add(kp.Value.Stringify(kp.Key));
            }

            table.AddRow(Row.Create(table, rowValues.ToArray()));
        }
    }
}