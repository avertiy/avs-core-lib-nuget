using AVS.CoreLib.Abstractions;

namespace AVS.CoreLib.Trading.Types
{
    public class XSymbol : IHasValue
    {
        public XSymbol(string symbol, string exchange)
        {
            Value = symbol;
            Exchange = exchange;
        }

        public string Value { get; set; }
        public string Exchange { get; set; }

        public static implicit operator string(XSymbol s)
        {
            return s.ToString();
        }

        public static implicit operator Symbol(XSymbol s)
        {
            return s.ToSymbol();
        }

        public Symbol ToSymbol()
        {
            return new Symbol(Value);
        }

        public override string ToString()
        {
            return $"{Exchange}:{Value}";
        }

        public bool HasValue => !string.IsNullOrEmpty(Value);

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
                return true;

            if (obj is XSymbol s)
                return s.Exchange == Exchange && s.Value == Value;

            return false;
        }
    }
}