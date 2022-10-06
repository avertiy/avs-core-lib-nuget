using System;
using System.Diagnostics;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Trading.Collections;
using AVS.CoreLib.Trading.Helpers;

namespace AVS.CoreLib.Trading.Types
{
    /// <summary>
    /// type to deal with a collection of symbols (trading instrument) <see cref="Symbol"/>   
    /// </summary>
    public class Symbols : StringCollection
    {
        public Symbols()
        {
        }


        public Symbols(params string[] items) : base(items)
        {
        }

        [DebuggerStepThrough]
        public static implicit operator Symbols(string symbols)
        {
            return Parse(symbols);
        }

        [DebuggerStepThrough]
        public static implicit operator Symbols(string[] symbols)
        {
            return new Symbols(symbols);
        }

        internal override string[] AllItems => TradingHelper.Instance.GetTopSymbols("BTC", "USD", "USDT", "USDC", "UAH");

        public static Symbols Parse(string str)
        {
            var res = new Symbols();
            if (string.IsNullOrEmpty(str))
                return res;
            res.Add(str.Either("all", "*") ? res.AllItems : str.ToUpper().Split(',', StringSplitOptions.RemoveEmptyEntries));
            return res;
        }
    }
}