using System.Diagnostics;
using AVS.CoreLib.Trading.Helpers;

namespace AVS.CoreLib.Trading.Types
{
    public class BaseCurrencies : StringCollection
    {
        public BaseCurrencies()
        {
        }

        public BaseCurrencies(params string[] items) : base(items)
        {
        }

        [DebuggerStepThrough]
        public static implicit operator BaseCurrencies(string exchanges)
        {
            return Parse<BaseCurrencies>(exchanges);
        }

        internal override string[] AllItems => TradingHelper.Instance.GetBaseCurrencies();
    }
}