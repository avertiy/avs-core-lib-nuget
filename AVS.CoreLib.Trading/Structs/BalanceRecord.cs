using System;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Trading.Structs
{
    public readonly struct BalanceRecord
    {
        public BalanceRecord(decimal @base, decimal quote)
        {
            Base = @base;
            Quote = quote;
        }

        public decimal Base { get; }
        public decimal Quote { get; }

        public decimal GetTotal(decimal price)
        {
            return (Base * price + Quote).Round();
        }

        public decimal GetDiff(BalanceRecord other, decimal price)
        {
            var total1 = GetTotal(price);
            var total2 = other.GetTotal(price);
            return total1 - total2;
        }

        public static BalanceRecord operator +(BalanceRecord value1, BalanceRecord value2)
        {
            var result = new BalanceRecord(value1.Base + value2.Base, value1.Quote + value2.Quote);
            return result;
        }
        
        public static BalanceRecord operator -(BalanceRecord value1, BalanceRecord value2)
        {
            var result = new BalanceRecord(value1.Base - value2.Base, value1.Quote - value2.Quote);
            return result;
        }
    }
}