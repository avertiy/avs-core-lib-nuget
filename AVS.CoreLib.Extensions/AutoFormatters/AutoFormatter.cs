using System;
using System.Collections;
using System.Collections.Generic;
using AVS.CoreLib.Extensions.Stringify;

namespace AVS.CoreLib.Extensions.AutoFormatters
{
    //AutoFormatter tmp sitting here, AutoFormatter + Stringify feature might be moved out into separate package..

    public interface IAutoFormatter
    {
        void AddFormatter<T>(Func<T, string> format);
        void AddFormatterByKeyword<T>(string keyword, Func<T, string> format);
        string Format(string key, object value);
        string Format(object value);
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
            Formatters.Register(DEFAULT_FORMATTER, x => x.ToString());
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

        protected virtual Func<object, string> PickFormatter(string key, object value)
        {
            //e.g. Timestamp
            if (Formatters.ContainsKey(key))
                return Formatters[key];

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
                if (str.Contains(kp.Key) && kp.Value.StartsWith(type.Name + ":"))
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