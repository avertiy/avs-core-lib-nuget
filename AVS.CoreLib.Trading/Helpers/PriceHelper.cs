using System;
using AVS.CoreLib.Trading.Extensions;
using AVS.CoreLib.Trading.Structs;
using AVS.CoreLib.Trading.Types;

namespace AVS.CoreLib.Trading.Helpers
{
    public static class PriceHelper
    {
        private static IPriceContainer _prices;

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

        public static decimal ConvertToUSDT(decimal total, string pair)
        {
            //total in BTC we need total in USDT
            if (pair.IsUsdPeggedPair())
                return total;

            var cp = new CurrencyPair(pair);
            var price = cp.BaseCurrency switch
            {
                "UAH" => Prices["UAH_USDT"],
                "RUB" => Prices["RUB_USDT"],
                _ => Prices["USDT_" + cp.BaseCurrency]
            };

            if (price <= 0)
                throw new ArgumentException($"USDT_{cp.BaseCurrency} price is not known");

            return total * price;
        }

        private static void AddIndicativePrices(this IPriceContainer prices)
        {
            // USD prices
            prices.Add("USDT_BTC", 6000);
            prices.Add("USDT_ADA", 0);
            prices.Add("USDT_ATOM", 0);
            prices.Add("USDT_BCH", 800);
            prices.Add("USDT_BNB", 500);
            prices.Add("USDT_BTT", 0);
            prices.Add("USDT_DASH", 300);
            prices.Add("USDT_DCR", 0);
            prices.Add("USDT_DOGE", 0.02m);
            prices.Add("USDT_DOT", 0);
            prices.Add("USDT_IOTA", 0);
            prices.Add("USDT_EOS", 0);
            prices.Add("USDT_ETH", 2500);
            prices.Add("USDT_JST", 0);
            prices.Add("USDT_LTC", 250);
            prices.Add("USDT_LSK", 4);
            prices.Add("USDT_NEO", 50);
            prices.Add("USDT_TRX", 0.1m);
            prices.Add("USDT_UNI", 0);
            prices.Add("USDT_XEM", 0);
            prices.Add("USDT_XLM", 0);
            prices.Add("USDT_XMR", 250);
            prices.Add("USDT_XRP", 1);
            prices.Add("USDT_WAVES", 11);
            prices.Add("USDT_ZEC", 0);
            prices.Add("USDT_ZRX", 10);
            prices.Add("USDT_OCEAN", 1);
            prices.Add("USDT_RIF", 0);
            prices.Add("USDT_REEF", 0);

            // BTC prices
            prices.Add("BTC_ADA", 0);
            prices.Add("BTC_ATOM", 0);
            prices.Add("BTC_BCH", 0.015m);
            prices.Add("BTC_BNB", 0);
            prices.Add("BTC_DASH", 0.0075m);
            prices.Add("BTC_DOGE", 0);
            prices.Add("BTC_DOT", 0);
            prices.Add("BTC_IOTA", 0);
            prices.Add("BTC_EOS", 0);
            prices.Add("BTC_ETH", 0.045m);
            prices.Add("BTC_LTC", 0.0045m);
            prices.Add("BTC_LSK", 0.00007m);
            prices.Add("BTC_TRX", 0.000002m);
            prices.Add("BTC_XEM", 0);
            prices.Add("BTC_XLM", 0);
            prices.Add("BTC_XMR", 0);
            prices.Add("BTC_XRP", 0.006m);
            prices.Add("BTC_WAVES", 0002m);
            prices.Add("BTC_ZEC", 0);

            prices.Add("UAH_USDT", 28);
            prices.Add("RUB_USDT", 74);

            //switch (cp.BaseCurrency)
            //{
            //    case "BTC":
            //        {
            //            switch (cp.QuoteCurrency)
            //            {
            //                case "BCH":
            //                case "DASH":
            //                    return 0.0075m;
            //                case "PTI":
            //                case "DOGE":
            //                    return 0.0000003m;
            //                case "ETH":
            //                    return 0.03m;
            //                case "LTC":
            //                    return 0.004m;
            //                case "XEM":
            //                    return 0.000005m;
            //                case "XLM":
            //                    return 0.00002m;
            //                case "XMR":
            //                    return 0.00075m;
            //                case "ADA":
            //                case "IOTA":
            //                case "XRP":
            //                case "ZRX":
            //                    return 0.000025m;
            //                case "ZEC":
            //                    return 0.005m;
            //                default:
            //                    return 0;
            //            }
            //        }

            //    case "USDC":
            //    case "USDT":
            //    case "USD":
            //    case "BUSD":
            //    case "PAX":
            //    case "DAI":
            //        {
            //            switch (cp.QuoteCurrency)
            //            {
            //                case "BTC":
            //                    return 40000;
            //                case "ETH":
            //                    return 1000;
            //                case "BCH":
            //                case "BSV":
            //                case "DASH":
            //                case "XMR":
            //                case "BNB":
            //                case "LTC":
            //                    return 250;
            //                case "ZEC":
            //                    return 100;
            //                case "NEO":
            //                    return 50;
            //                case "ATOM":
            //                case "EOS":
            //                case "DOT":
            //                case "DCR":
            //                case "XTZ":
            //                case "WAVES":
            //                    return 10;
            //                case "USDT":
            //                case "USDC":
            //                case "PAX":
            //                case "DAI":
            //                case "USD":
            //                case "LSK":
            //                    return 1;
            //                case "ADA":
            //                case "IOTA":
            //                case "XRP":
            //                case "ZRX":
            //                    return 0.5m;
            //                case "TRX":
            //                case "XEM":
            //                case "XLM":
            //                    return 0.1m;
            //                case "DOGE":
            //                case "BTT":
            //                    return 0.005m;
            //                default:
            //                    return 0;
            //            }
            //        }
            //    case "UAH":
            //    case "RUB":
            //        {
            //            switch (cp.QuoteCurrency)
            //            {
            //                case "BTC":
            //                    return 250000;
            //                case "BCH":
            //                case "BSV":
            //                case "ETH":
            //                    return 5000;
            //                case "DASH":
            //                case "XMR":
            //                case "ZEC":
            //                    return 1500;
            //                case "BNB":
            //                case "LTC":
            //                case "NEO":
            //                    return 1000;
            //                case "ATOM":
            //                case "EOS":
            //                case "DOT":
            //                case "DCR":
            //                case "XTZ":
            //                case "WAVES":
            //                    return 100;
            //                case "USDT":
            //                case "USDC":
            //                case "PAX":
            //                case "DAI":
            //                case "USD":
            //                case "LSK":
            //                    return 25;
            //                case "ADA":
            //                case "IOTA":
            //                case "XRP":
            //                case "ZRX":
            //                    return 5;
            //                case "TRX":
            //                case "XEM":
            //                case "XLM":
            //                    return 1;
            //                case "PTI":
            //                case "DOGE":
            //                case "BTT":
            //                    return 0.05m;
            //                default:
            //                    return 0;
            //            }
            //        }
            //    default:
            //        return 0;
            //}
        }
    }
}