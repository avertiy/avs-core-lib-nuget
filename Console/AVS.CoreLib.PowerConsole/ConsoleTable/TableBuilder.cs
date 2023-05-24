using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.Extensions.Stringify;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public class TableBuilder
    {
        public ColumnOptions ColumnOptions { get; set; }
        public string[]? ExcludeProperties { get; set; }

        

        public Table CreateTable<T>(IEnumerable<T> data, string? title = null)
        {
            var table = new Table() { Title = title };
            var type = typeof(T);
            var allProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var props = allProperties.Where(p => p.CanRead && FilterProperties(p.Name)).ToArray();

            if (!props.Any())
                return table;

            table.Columns = props.Select(pi => new Column() { Title = pi.Name }).ToList();

            foreach (var obj in data)
            {
                table.AddRow(obj, props);
            }

            table.CalculateWidth();
            return table;
        }

        public Table CreateTable(object source)
        {
            Guard.Against.Null(source, "Source is missing");

            var type = source.GetType();
            var table = new Table() { Title = type.Name };
            var dict = type.Reflect(source!, FilterProperties);

            if (ColumnOptions == ColumnOptions.Auto)
            {
                if (dict.Count <= 20)
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

        private void BuildHorizontal(Table table, Dictionary<string, object> propValues)
        {
            var width = 0;
            var keys = new List<string>(propValues.Count);
            var values = new List<string>(propValues.Count);
            
            foreach (var kp in propValues)
            {
                var str = kp.Value.Stringify(kp.Key);
                keys.Add(kp.Key);
                values.Add(str);
                width += kp.Key.Length > str.Length ? kp.Key.Length : str.Length;
                width += 2;
            }


            if (width <= 160)
            {
                table.AddColumns(keys.ToArray());
                table.AddRow(Row.Create(table, values.ToArray()));
            }
            else
            {
                var count = keys.Count / 3;
                int n = Math.Max(width / 100 + 2, count+1);
                
                var slicedKeys = keys.Slice(n).ToArray();
                var slicedValues = values.Slice(n).ToArray();
                
                for (var j = 0; j < n; j++)
                {
                    table.AddRow(new Row());
                }

                for (var i = 0; i < slicedKeys.Length; i++)
                {
                    for (var j = 0; j < n && j<slicedKeys[i].Length; j++)
                    {
                        var key = slicedKeys[i][j];
                        var val = slicedValues[i][j];

                        table.Rows[j].AddCell(key);
                        table.Rows[j].AddCell(val.Trim());
                    }
                }
            }
        }

        private bool FilterProperties(string prop, object value)
        {
            var skip = ExcludeProperties != null && ExcludeProperties.Contains(prop) || value.IsEmpty();
            return !skip;
        }

        private bool FilterProperties(string prop)
        {
            var skip = ExcludeProperties != null && ExcludeProperties.Contains(prop);
            return !skip;
        }
    }
}