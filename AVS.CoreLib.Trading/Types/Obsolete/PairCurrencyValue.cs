using System;
using AVS.CoreLib.Trading.Exceptions;
using AVS.CoreLib.Trading.Extensions;
using AVS.CoreLib.Trading.Helpers;

namespace AVS.CoreLib.Trading.Types
{
    [Obsolete("looks not much usable type..")]
    public class PairCurrencyValue
    {
        public string Pair { get; }

        private decimal _quote;
        private decimal _base;

        public decimal Quote => _quote;
        public decimal Base => _base;

        public PairCurrencyValue(string pair)
        {
            Pair = pair;
            _quote = 0;
            _base = 0;
        }

        public PairCurrencyValue(string pair, decimal quote, decimal @base)
        {
            Pair = pair;
            _quote = quote;
            _base = @base;
        }

        public void DebitQuote(decimal amount)
        {
            if (Quote < amount)
                throw new NotEnoughBalanceException($"Debit of {amount:N3} {Pair.QuoteCurrency()} is not possible");
            _quote -= amount;
        }

        public void DebitBase(decimal amount)
        {
            if (Base < amount)
                throw new NotEnoughBalanceException($"Debit of {amount:N3} {Pair.BaseCurrency()} is not possible");
            _base -= amount;
        }

        public void CreditQuote(decimal amount)
        {
            _quote += amount;
        }

        public void CreditBase(decimal amount)
        {
            _base += amount;
        }

        public override string ToString()
        {
            var cp = new CurrencyPair(Pair);
            return $"{Quote.FormatNumber()} {cp.QuoteCurrency}; {Base.FormatNumber()} {cp.BaseCurrency}";
        }

        public static CurrencyValue operator +(PairCurrencyValue pb, CurrencyValue balance)
        {
            if (pb.Pair.QuoteCurrency() == balance.Currency)
                pb.CreditQuote(balance.Value);

            if (pb.Pair.BaseCurrency() == balance.Currency)
                pb.CreditBase(balance.Value);

            throw new ArgumentException($"Unable to do operation on {pb.Pair} with {balance.Currency}");
        }

        public static CurrencyValue operator -(PairCurrencyValue pb, CurrencyValue balance)
        {
            if (pb.Pair.QuoteCurrency() == balance.Currency)
                pb.DebitQuote(balance.Value);

            if (pb.Pair.BaseCurrency() == balance.Currency)
                pb.DebitBase(balance.Value);

            throw new ArgumentException($"Unable to do operation on {pb.Pair} with {balance.Currency}");
        }

        /// <summary>
        /// parses values like 0.2 BTC;10 USD into corresponding PairAmount value 
        /// </summary>
        public static PairCurrencyValue Parse(string str)
        {
            var parts = str.Split(';', ' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4)
                throw new ArgumentException($"String '{str}' is not recognized as a valid {nameof(PairCurrencyValue)} string (e.g. 0.2 BTC; 10 USD)");

            if (NumericHelper.TryParseDecimal(parts[0], out var quote) && NumericHelper.TryParseDecimal(parts[2], out var @base))
            {
                var pair = new CurrencyPair(parts[1], parts[3]);
                return new PairCurrencyValue(pair.ToString(), quote, @base);
            }

            throw new ArgumentException($"Unable to parse '{str}' into {nameof(PairCurrencyValue)} value");
        }

        public static implicit operator PairCurrencyValue(string str)
        {
            return Parse(str);
        }

        public static implicit operator string(PairCurrencyValue pairValue)
        {
            return pairValue.Pair;
        }
    }
}