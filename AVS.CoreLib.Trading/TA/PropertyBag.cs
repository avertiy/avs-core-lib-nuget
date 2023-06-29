#nullable enable
using System.Collections.Generic;
using AVS.CoreLib.Trading.Abstractions.XBars;
using AVS.CoreLib.Trading.Enums.TA;

namespace AVS.CoreLib.Trading.TA
{
    public interface IPropertyBag
    {
        object? this[string key] { get; set; }
    }
    public class PropertyBag : IPropertyBag
    {
        private readonly Dictionary<string, object?> _values;

        public PropertyBag(int capacity = 5)
        {
            _values = new Dictionary<string, object?>(capacity);
        }

        public void Set(string key, object? value)
        {
            _values[key] = value;
        }

        public object? Get(string key)
        {
            return _values.ContainsKey(key) ? _values[key] : null;
        }

        public object? this[string key]
        {
            get => Get(key);
            set => Set(key, value);
        }
    }

    public static class TAExtensions
    {
        public static IMAValue? MA(this IPropertyBag bag, int length, MAType type = MAType.SMA)
        {
            return (IMAValue?)bag[$"{type}{length}"];
        }

        public static IMAValue? SMA(this IPropertyBag bag, int length)
        {
            return (IMAValue?)bag[$"{nameof(SMA)}{length}"];
        }

        public static IBBValue? BB(this IPropertyBag bag, int length)
        {
            return (IBBValue?)bag[$"{nameof(BB)}{length}"];
        }
    }
}