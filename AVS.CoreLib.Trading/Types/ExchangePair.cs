using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Trading.Structs;

namespace AVS.CoreLib.Trading.Types
{
    public class ExchangePair : IHasValue
    {
        public ExchangePair(string pair, string exchange, bool isBaseCurrencyFirst = true)
        {
            Value = pair;
            Exchange = exchange;
            IsBaseCurrencyFirst = isBaseCurrencyFirst;
        }

        public string Value { get; set; }
        public string Exchange { get; set; }
        public bool IsBaseCurrencyFirst { get; set; }

        public static implicit operator string(ExchangePair s)
        {
            return s.IsBaseCurrencyFirst ? s.Value : s.Value.Swap('_');
        }

        public static implicit operator PairString(ExchangePair s)
        {
            return s.ToPairString();
        }

        public PairString ToPairString()
        {
            var pair = IsBaseCurrencyFirst ? Value : Value.Swap('_');
            return new PairString(pair);
        }

        public override string ToString()
        {
            return $"{Exchange}/{Value}";
        }

        public bool HasValue => !string.IsNullOrEmpty(Value);

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
                return true;

            if (obj is ExchangePair pair)
            {
                return pair.Exchange == Exchange && pair.Value == Value &&
                       pair.IsBaseCurrencyFirst == IsBaseCurrencyFirst;
            }

            return false;
        }
    }
}