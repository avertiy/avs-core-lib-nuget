using System;
using System.Linq;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.Extensions;
using AVS.CoreLib.Trading.Helpers;
using AVS.CoreLib.Trading.Types;

namespace AVS.CoreLib.Trading.Collections
{
    public class CurrencyCollection : StringCollection
    {
        public CurrencyCollection()
        {
        }

        public CurrencyCollection(params string[] items) : base(items)
        {
        }

        public bool Any { get; set; }

        public CryptoCategory? Category { get; set; }

        public bool IsAllOrAny()
        {
            return Any || Category.HasValue && Category.Value == CryptoCategory.All;
        }

        public bool MatchQuoteCurrency(string symbol)
        {
            if (IsAllOrAny())
                return true;

            return Contains(symbol.GetQuoteCurrency());
        }

        public bool MatchBaseCurrency(string symbol)
        {
            if (IsAllOrAny())
                return true;

            return Contains(symbol.GetBaseCurrency());
        }

        //[DebuggerStepThrough]
        public static implicit operator CurrencyCollection(string currencies)
        {
            var res = new CurrencyCollection();

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
            return Category.HasValue ? TradingHelper.Instance.GetCurrencies(Category.Value) : Items.ToArray();
        }

        internal override string[] AllItems => Array.Empty<string>();
    }
}