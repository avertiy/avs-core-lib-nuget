using System;
using System.Diagnostics;
using System.Linq;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.Helpers;
using AVS.CoreLib.Trading.Structs;

namespace AVS.CoreLib.Trading.Types
{
    public class Currencies : StringCollection
    {
        public Currencies()
        {
        }

        public Currencies(params string[] items) : base(items)
        {
        }

        public bool Any { get; set; }

        public CryptoCategory? Category { get; set; }

        public bool IsAllOrAny()
        {
            return Any || Category.HasValue && Category.Value == CryptoCategory.All;
        }

        public bool MatchPair(string pair)
        {
            if (IsAllOrAny())
                return true;
            var cp = new CurrencyPair(pair);
            return Contains(cp.QuoteCurrency);
        }

        //[DebuggerStepThrough]
        public static implicit operator Currencies(string currencies)
        {
            var res = new Currencies();

            if (string.IsNullOrEmpty(currencies))
                return res;

            if (Enum.TryParse(currencies, out CryptoCategory category))
            {
                res.Category = category;
                return res;
            }

            if (currencies.Either("any", "*"))
            {
                res.Any = true;
                return res;
            }

            if (currencies == "all")
            {
                res.Category = CryptoCategory.All;
                return res;
            }

            res.Add(currencies.Split(',', StringSplitOptions.RemoveEmptyEntries));
            return res;
        }

        public string[] ToArray()
        {
            return Category.HasValue ? TradingHelper.Instance.GetCurrencies(Category.Value) : this.Items.ToArray();
        }

        internal override string[] AllItems => Array.Empty<string>(); 
    }
}