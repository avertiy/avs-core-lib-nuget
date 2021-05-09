using System.Collections.Generic;
using System.Diagnostics;
using AVS.CoreLib.Trading.Helpers;
using AVS.CoreLib.Trading.Structs;

namespace AVS.CoreLib.Trading.Types
{
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

        public bool MatchPair(string pair)
        {
            var cp = new CurrencyPair(pair);
            return Contains(cp.BaseCurrency);
        }

        internal override string[] AllItems => TradingHelper.Instance.GetBaseCurrencies();
    }
}