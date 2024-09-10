using System;
using System.Collections.Generic;
using System.Reflection;

namespace AVS.CoreLib.Extensions.AutoFormatters
{
    //AutoFormatter tmp sitting here, AutoFormatter + Stringify feature might be moved out into separate package..

    public interface IArgumentFormatProvider
    {
        /// <summary>
        /// format object argument
        /// <code>
        ///     Format(150.00, new ArgumentDescriptor(){ ... }) => &lt;Green&gt;$150.00&lt;/Green&gt;
        /// </code>
        /// </summary>
        /// <returns>
        /// formatted value (it might contain color tags / ansi codes)
        /// </returns>
        string Format(object arg, ArgumentDescriptor descriptor);
    }

    //to calculate Cell width we would still need text without color tags due to we calc. str.length
    //thus it might need to either do separate format value 155.00 => $155.00 than colorize
    //or return some struct that contains text and color and colors will be applied applied already by printer
    public class ArgumentDescriptor
    {
        /// <summary>
        /// .raw value
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// hold format modifier e.g. {arg:C} => `C` or {arg:### ###.00} => `### ###.00`
        /// </summary>
        public string? Format { get; set; }
        /// <summary>
        /// formatted value e.g. $150.00 in case we need formatted value length to calc cell width for Table
        /// (no color tags ansi-codes etc.)
        /// </summary>
        public string? FormattedValue { get; set; }

        //public Colors? Colors { get; set; }
        //public ObjType PrimitiveType // null/string/bool/float/int/array/list/dict/other
        //public FormatFlags Flags // zero/empty/negative/currency/percentage/short string/text/json

        public PropertyInfo? Property { get; set; }
        public string? PropertyName => Property?.Name;

        public Type? Type { get; set; }
        public Type? OwnerType { get; set; }
    }

    public interface IAutoFormatter
    {
        IFormatterRegistry Formatters { get; }

        void AddFormatter<T>(Func<T, string> format);
        /// <summary>
        /// Add special formatter by keyword in type/property name
        /// </summary>
        /// <param name="keyword">keyword case sensitivity is off</param>
        /// <param name="format">formatter func</param>
        void AddFormatterByKeyword<T>(string keyword, Func<T, string> format);
        string Format(string key, object value);
        string Format(object value);
        bool Contains(string keyOrKeyword);

        void AddKeywordMapping(string keyword, string key);
    }

    /// <summary>
    /// AutoFormatter the idea is to define generic common formatters for types & properties so when we need to print object or log it or stringify
    /// we avoid a huge routine
    /// try PowerConsole.PrintObject to see it in action
    /// </summary>
    public class AutoFormatter : IAutoFormatter
    {
        public const string DEFAULT_FORMATTER = "_DEFAULT_";
        private static IAutoFormatter? _instance;
        public static IAutoFormatter Instance
        {
            get => _instance ??= new AutoFormatter().AddBaseFormatters().AddFinancialFormatters();
            set => _instance = value;
        }

        public IFormatterRegistry Formatters { get; set; } = new FormatterRegistry();
        /// <summary>
        /// keyword allows to use match keys by keyword & value type
        /// </summary>
        private readonly Dictionary<string, string> _keywordMap = new();

        public AutoFormatter()
        {
            Formatters.Register(DEFAULT_FORMATTER, x => x?.ToString() ?? "");
        }

        public void AddFormatter<T>(Func<T, string> format)
        {
            Formatters.Register(typeof(T).Name, x => format((T)x));
        }

        public void AddFormatterByKeyword<T>(string keyword, Func<T, string> format)
        {
            var type = typeof(T);
            var key = $"{type.Name}:{keyword.ToLower()}";
            Formatters.Register(key, x => format((T)x));
            _keywordMap.Add(keyword, key);
        }

        public void AddKeywordMapping(string keyword, string key)
        {
            if (!Formatters.ContainsKey(key))
                throw new KeyNotFoundException(key);

            _keywordMap.Add(keyword, key);
        }

        public bool Contains(string keyOrKeyword)
        {
            return _keywordMap.ContainsKey(keyOrKeyword) || Formatters.ContainsKey(keyOrKeyword);
        }

        /// <summary>
        /// format single object value (not enumerable/array/list/etc.)
        /// tries to pick formatter by key, then by type name, then fall back format with ToString()
        /// </summary>
        public string Format(string key, object value)
        {
            var formatter = PickFormatter(key, value);
            return formatter(value);
        }

        public string Format(object value)
        {
            var type = value.GetType();
            var key = type.Name;
            var formatter = Formatters.GetFormatterOrDefault(key, DEFAULT_FORMATTER);
            return formatter(value);
        }

        protected virtual Func<object, string> PickFormatter(string key, object? value)
        {
            //e.g. Timestamp
            if (Formatters.ContainsKey(key))
                return Formatters[key];

            if (value == null)
                return Formatters[DEFAULT_FORMATTER];

            var type = value.GetType();
            // e.g. EntryPrice
            if (Match(key, type, out var specialFormatterKey))
            {
                return Formatters[specialFormatterKey];
            }

            // e.g. DateTime
            var formatter = Formatters.GetFormatterOrDefault(type.Name, DEFAULT_FORMATTER);
            return formatter;
        }

        protected bool Match(string propName, Type type, out string specialFormatterKey)
        {
            var str = propName.ToLower();
            foreach (var kp in _keywordMap)
            {
                if (!str.Contains(kp.Key))
                    continue;

                if (kp.Value.StartsWith(type.Name + ":") || kp.Value.StartsWith(type.BaseType?.Name + ":"))
                {
                    specialFormatterKey = kp.Value;
                    return true;
                }
            }

            specialFormatterKey = default!;
            return false;
        }
    }
}