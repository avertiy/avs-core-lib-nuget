using System;
using System.Collections.Generic;
using System.Diagnostics;
using AVS.CoreLib.Trading.Collections;
using AVS.CoreLib.Trading.Helpers;
using AVS.CoreLib.Trading.Structs;

namespace AVS.CoreLib.Trading.Types
{
    [Obsolete("Use CurrencyCollection instead")]
    public class BaseCurrencies : StringCollection
    {
        public BaseCurrencies()
        {
        }

        public BaseCurrencies(IList<string> items) : base(items)
        {
        }

        [DebuggerStepThrough]
        public static implicit operator BaseCurrencies(string exchanges)
        {
            return Parse<BaseCurrencies>(exchanges);
        }

        public bool MatchQuote(string symbol)
        {
            var cp = new CurrencyPair(symbol);
            return Contains(cp.QuoteCurrency);
        }

        internal override string[] AllItems => TradingHelper.Instance.GetBaseCurrencies();
    }
}