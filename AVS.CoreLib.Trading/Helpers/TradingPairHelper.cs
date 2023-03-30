using System.Collections.Generic;

namespace AVS.CoreLib.Trading.Helpers
{
    /// <summary>
    /// helps to generate trading pairs (aka symbols) combining quote and base currencies
    /// </summary>
    public static class TradingPairHelper
    {
        /// <summary>
        /// combines all quote assets with all base assets
        /// e.g. ["USDT","BUSD"] + ["BTC","XRP"] => ["BTC_USDT","BTC_BUSD", "XRP_USDT","XRP_BUSD"]
        /// </summary>
        public static string[] CombineAll(string[] quoteCurrencies, string[] baseCurrencies)
        {
            var symbols = new List<string>();
            foreach (var baseCurrency in baseCurrencies)
            {
                foreach (var quoteCur in quoteCurrencies)
                {
                    if (quoteCur == baseCurrency)
                        continue;
                    symbols.Add(baseCurrency + "_" + quoteCur);
                }
            }
            return symbols.ToArray();
        }

        /// <summary>
        /// combines base asset with quote asset(s)
        /// e.g. "BTC" + ["USDT","BUSD"] => ["BTC_USDT","BTC_BUSD"]
        /// </summary>
        public static string[] CombineWithQuoteAssets(string baseCurrency, params string[] quoteCurrencies)
        {
            var symbols = new List<string>();
            foreach (var quoteCur in quoteCurrencies)
            {
                if (quoteCur == baseCurrency)
                    continue;
                symbols.Add(baseCurrency + "_" + quoteCur);
            }
            return symbols.ToArray();
        }

        /// <summary>
        /// combine <see cref="quoteCurrency"/> with <see cref="baseCurrencies"/>
        /// e.g. USDT + [BTC,ETH,XRP] => [BTC_USDT, ETH_USDT, XRP_USDT]
        /// </summary>
        public static string[] Combine(string quoteCurrency, params string[] baseCurrencies)
        {
            var symbols = new List<string>();
            foreach (var baseCurr in baseCurrencies)
            {
                if (baseCurr == quoteCurrency)
                    continue;

                symbols.Add(baseCurr + "_" + quoteCurrency);
            }
            return symbols.ToArray();
        }
    }
}