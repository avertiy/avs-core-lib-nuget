using System.Diagnostics;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.Helpers;

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

        [DebuggerStepThrough]
        public static implicit operator Currencies(string exchanges)
        {
            return Parse<Currencies>(exchanges);
        }

        internal override string[] AllItems => TradingHelper.Instance.GetCurrencies(CryptoCategory.AllCrypto);
    }
}