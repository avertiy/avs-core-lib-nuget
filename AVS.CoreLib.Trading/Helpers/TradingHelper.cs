using System.Collections.Generic;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Helpers
{
    public interface ITradingHelper
    {
        string[] GetAllExchanges();
        string[] GetBaseCurrencies(bool all = true);
        string[] GetFiatCurrencies(CryptoCategory currencies = CryptoCategory.Fiat);
        string[] GetTopPairs(params string[] baseCurrencies);
        string[] GetCurrencies(CryptoCategory currencies = CryptoCategory.AllCrypto);
    }

    public class TradingHelper : ITradingHelper
    {
        public static ITradingHelper Instance = new TradingHelper();
        public string[] GetAllExchanges()
        {
            return new[] { "Binance", "Bitfinex", "Bitstamp", "Bittrex", "CoinBase", "Exmo", "HitBtc", "Huobi", "Kraken", "KuCoin", "Kuna", "Liquid", "Poloniex" };
        }

        public string[] GetCurrencies(CryptoCategory currencies = CryptoCategory.AllCrypto)
        {
            var list = new List<string>();

            if (currencies.HasFlag(CryptoCategory.Top20))
                list.AddRange(new[] { "BCH", "BTC", "DASH", "DOGE", "ETH", "LTC", "XLM", "XEM", "XMR", "XRP" });

            if (currencies.HasFlag(CryptoCategory.StableCoin))
                list.AddRange(new[] { "USDT", "USDC", "BUSD", "DAI", "TUSD", "PAX" });

            if (currencies.HasFlag(CryptoCategory.Fiat))
                list.AddRange(new[] { "USD", "UAH", "EUR", "RUB", "AUD", "CAD", "GBP", "INR", "IDR", "PHP", "SGD", "SNI" });

            //if (currencies.HasFlag(CryptoCategory.Tradeable))
            //    list.AddRange(new[] { "ADA", "ATOM", "BTT", "EOS", "EXM", "IOTA", "LSK", "NEO", "PTI", "QTUM", "TRX", "XTZ", "WAVES", "ZEC", "ZRX" });

            return list.ToArray();
        }

        public string[] GetBaseCurrencies(bool all = true)
        {
            return all ?
                new[] { "BTC", "ETH", "USDT", "USDC", "USD", "UAH", "EUR", "RUB", "GBP", "CAD" } :
                new[] { "BTC", "ETH", "USDT", "USDC", "USD", "UAH" };
        }

        public string[] GetFiatCurrencies(CryptoCategory currencies = CryptoCategory.Fiat)
        {
            var list = new List<string>();
            if (currencies.HasFlag(CryptoCategory.Fiat))
                list.AddRange(new[] { "USD", "UAH", "EUR", "RUB", "AUD", "CAD", "GBP", "INR", "IDR", "PHP", "SGD", "SNI" });

            return list.ToArray();
        }

        public string[] GetTopPairs(params string[] baseCurrencies)
        {
            var res = new List<string>();

            foreach (var baseCurrency in baseCurrencies)
            {
                switch (baseCurrency)
                {
                    case "BTC":
                        {
                            res.AddRange(PairHelper.GeneratePairs("BTC", "BCH", "DASH", "DOGE", "ETH", "LTC", "XEM", "XLM", "XMR", "XRP", "ZEC"));
                            break;
                        }
                    case "USD":
                        {
                            res.AddRange(PairHelper.GeneratePairs("USD", "BTC", "BCH", "DASH", "DOGE", "ETH", "LTC", "XEM", "XLM", "XMR", "XRP", "USDT", "USDC", "ZEC"));
                            break;
                        }

                    case "USDT":
                        {
                            res.AddRange(PairHelper.GeneratePairs("USDT", "BTC", "BCH", "DASH", "DOGE", "ETH", "LTC", "XEM", "XLM", "XMR", "XRP", "ZEC"));
                            break;
                        }
                    case "USDC":
                        {
                            res.AddRange(PairHelper.GeneratePairs("USDC", "BTC", "BCH", "DASH", "DOGE", "ETH", "LTC", "XEM", "XLM", "XMR", "XRP"));
                            break;
                        }
                    case "UAH":
                        {
                            res.AddRange(PairHelper.GeneratePairs("UAH", "BTC", "BCH", "DASH", "ETH", "LTC", "XEM", "XLM", "XRP", "USDT", "ZEC"));
                            break;
                        }
                }
            }

            return res.ToArray();
        }
    }
}