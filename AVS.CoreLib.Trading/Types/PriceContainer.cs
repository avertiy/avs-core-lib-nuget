using System.Collections.Generic;

namespace AVS.CoreLib.Trading.Types
{
    public interface IPriceContainer
    {
        //decimal BtcUsd { get; set; }
        decimal this[string pair] { get; set; }

        void Add(string pair, decimal price);
        void Update(IDictionary<string, decimal> prices);

        bool Contains(string pair);
    }

    public class PriceContainer : IPriceContainer
    {
        public IDictionary<string, decimal> Items { get; }

        public PriceContainer(IDictionary<string, decimal> prices = null)
        {
            Items = new Dictionary<string, decimal>();
            Update(prices);
        }

        public decimal BtcUsd
        {
            get => this["USDT_BTC"];
            set => this["USDT_BTC"] = value;
        }

        public decimal this[string pair]
        {
            get => Items.ContainsKey(pair) ? Items[pair] : 0;
            set => Items[pair] = value;
        }

        public void Add(string pair, decimal price)
        {
            Items.Add(pair, price);
        }

        public void Update(IDictionary<string, decimal> prices)
        {
            if (prices == null || prices.Count == 0)
                return;

            foreach (var kp in prices)
                Items[kp.Key] = kp.Value;
        }

        public bool Contains(string pair)
        {
            return Items.ContainsKey(pair);
        }
    }
}