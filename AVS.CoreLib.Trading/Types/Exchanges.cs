using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AVS.CoreLib.Trading.Helpers;

namespace AVS.CoreLib.Trading.Types
{
    public class Exchanges : StringCollection
    {
        public Exchanges()
        {
        }

        public Exchanges(IList<string> items) : base(items)
        {
        }

        [DebuggerStepThrough]
        public static implicit operator Exchanges(string exchanges)
        {
            return Parse<Exchanges>(exchanges);
        }

        public Exchanges Clone()
        {
            var exchanges = new Exchanges();
            foreach (var item in Items)
                exchanges.Add(item);
            return exchanges;
        }

        internal override string[] AllItems => TradingHelper.Instance.GetAllExchanges();

        public static bool IsExchange(string name)
        {
            return TradingHelper.Instance.GetAllExchanges().Contains(name);
        }
    }
}