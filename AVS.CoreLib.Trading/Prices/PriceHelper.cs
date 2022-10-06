using System;
using AVS.CoreLib.Trading.Extensions;
using AVS.CoreLib.Trading.Types;

namespace AVS.CoreLib.Trading.Prices
{
    public static class PriceHelper
    {
        private static IPriceContainer _prices;

        /// <summary>
        /// these are indicative prices for purposes like ranking coins by value relative to other coins
        /// indicative means for example BTC is ~10 times bigger than ETH or ~100 times bigger than BCH, LTC etc. 
        /// </summary>
        public static IPriceContainer Prices
        {
            get
            {
                if (_prices == null)
                {
                    _prices = new PriceContainer();
                    _prices.AddIndicativePrices();
                }
                return _prices;
            }
            set => _prices = value;
        }

        public static decimal ConvertToUSDT(decimal total, string symbol)
        {
            //total in BTC we need total in USDT
            if (symbol.IsUsdPeggedPair())
                return total;

            var cp = new CurrencyPair(symbol);
            var price = cp.QuoteCurrency switch
            {
                "UAH" => Prices["USDT_UAH"],
                "RUB" => Prices["USDT_RUB"],
                "EUR" => Prices["USDT_EUR"],
                _ => Prices["USDT_" + cp.BaseCurrency]
            };

            if (price <= 0)
                throw new ArgumentException($"{cp.BaseCurrency}_USDT price is not known");

            return total * price;
        }


        private static void AddIndicativePrices(this IPriceContainer prices)
        {
            //these are indicative prices i.e. 

            // USD prices
            prices.Add("BTC_USDT", 20000);
            prices.Add("ADA_USDT", 0);
            prices.Add("BCH_USDT", 200);
            prices.Add("BNB_USDT", 280);
            prices.Add("BTT_USDT", 0);
            prices.Add("DCR_USDT", 0);
            prices.Add("DOT_USDT", 0);
            prices.Add("EOS_USDT", 0);
            prices.Add("ETH_USDT", 2000);
            prices.Add("JST_USDT", 0);
            prices.Add("LTC_USDT", 50);
            prices.Add("LSK_USDT", 1);
            prices.Add("NEO_USDT", 50);
            prices.Add("RIF_USDT", 0);
            prices.Add("TRX_USDT", 0.1m);
            prices.Add("UNI_USDT", 0);
            prices.Add("XEM_USDT", 0);
            prices.Add("XLM_USDT", 0);
            prices.Add("XMR_USDT", 250);
            prices.Add("XRP_USDT", 0.5m);
            prices.Add("ZEC_USDT", 0);
            prices.Add("ZRX_USDT", 10);

            prices.Add("ATOM_USDT", 0);
            prices.Add("DASH_USDT", 300);
            prices.Add("DOGE_USDT", 0.02m);
            prices.Add("IOTA_USDT", 0);
            prices.Add("REEF_USDT", 0);

            prices.Add("WAVES_USDT", 11);
            prices.Add("OCEAN_USDT", 1);

            // BTC prices
            prices.Add("ADA_BTC", 0.00002m);
            prices.Add("BCH_BTC", 0.015m);
            prices.Add("BNB_BTC", 0.014m);
            prices.Add("DOT_BTC", 0.003m);
            prices.Add("EOS_BTC", 0);
            prices.Add("ETH_BTC", 0.05m);
            prices.Add("LTC_BTC", 0.0025m);
            prices.Add("LSK_BTC", 0.00004m);
            prices.Add("TRX_BTC", 0.0000025m);
            prices.Add("XEM_BTC", 0);
            prices.Add("XLM_BTC", 0);
            prices.Add("XMR_BTC", 0);
            prices.Add("XRP_BTC", 0.00003m);
            prices.Add("ZEC_BTC", 0);

            prices.Add("ATOM_BTC", 0.0006m);
            prices.Add("DASH_BTC", 0.0025m);
            prices.Add("DOGE_BTC", 0);
            prices.Add("IOTA_BTC", 0);
            prices.Add("WAVES_BTC", 0.0002m);

            prices.Add("USDT_UAH", 40);
            prices.Add("USDT_RUB", 100);
        }
    }
}