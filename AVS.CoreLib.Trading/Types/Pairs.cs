using System;
using System.Diagnostics;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Trading.Helpers;

namespace AVS.CoreLib.Trading.Types
{
    [Obsolete("Use Symbols instead")]
    public class Pairs : StringCollection
    {
        public Pairs()
        {
        }


        public Pairs(params string[] items) : base(items)
        {
        }

        [DebuggerStepThrough]
        public static implicit operator Pairs(string pairs)
        {
            return Parse(pairs);
        }

        [DebuggerStepThrough]
        public static implicit operator Pairs(string[] pairs)
        {
            return new Pairs(pairs);
        }

        internal override string[] AllItems => TradingHelper.Instance.GetTopPairs("BTC", "USD", "USDT", "USDC", "UAH");

        public static Pairs Parse(string str)
        {
            var res = new Pairs();
            if (string.IsNullOrEmpty(str))
                return res;
            res.Add(str.Either("all", "*") ? res.AllItems : str.ToUpper().Split(',', StringSplitOptions.RemoveEmptyEntries));
            return res;
        }
    }
}