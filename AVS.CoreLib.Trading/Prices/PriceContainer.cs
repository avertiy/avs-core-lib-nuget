using System.Collections.Generic;

namespace AVS.CoreLib.Trading.Prices
{
    public interface IPriceContainer
    {
        //decimal BtcUsd { get; set; }
        decimal this[string symbol] { get; set; }
        void Add(string symbol, decimal price);
        void Update(IDictionary<string, decimal> prices);
        bool Contains(string symbol);
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
            get => this["BTC_USDT"];
            set => this["BTC_USDT"] = value;
        }

        public decimal this[string symbol]
        {
            get => Items.ContainsKey(symbol) ? Items[symbol] : 0;
            set => Items[symbol] = value;
        }

        public void Add(string symbol, decimal price)
        {
            Items.Add(symbol, price);
        }

        public void Update(IDictionary<string, decimal> prices)
        {
            if (prices == null || prices.Count == 0)
                return;

            foreach (var kp in prices)
                Items[kp.Key] = kp.Value;
        }

        public bool Contains(string symbol)
        {
            return Items.ContainsKey(symbol);
        }
    }
}