using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Helpers
{
    [Obsolete("not bad idea but seems needs to be reworked avoid using it")]
    public interface ITradingHelper
    {
        string[] GetAllExchanges();
        string[] GetBaseCurrencies(bool all = true);
        string[] GetFiatCurrencies(CryptoCategory currencies = CryptoCategory.Fiat);
        [Obsolete("use GetTopSymbols")]
        string[] GetTopPairs(params string[] baseCurrencies);
        string[] GetTopSymbols(params string[] quoteCurrencies);
        string[] GetCurrencies(CryptoCategory currencies = CryptoCategory.AllCrypto);
    }

    [Obsolete("not bad idea but seems needs to be reworked avoid using it")]
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

            if (currencies.HasFlag(CryptoCategory.Top))
                list.AddRange(new[]
                {
                    "ADA",
                    "BNB", "BTC", "BCH",
                    "DOGE", "DOT",
                    "EOS", "ETH",
                    "LTC",
                    "NEO",
                    "TRX",
                    "XEM", "XLM", "XMR", "XRP",
                    "ZEC",
                });

            if (currencies.HasFlag(CryptoCategory.StableCoin))
                list.AddRange(new[] { "USDT", "USDC", "BUSD", "DAI", "HUSD", "USDJ", "TUSD", "PAX" });

            if (currencies.HasFlag(CryptoCategory.DeFi))
                list.AddRange(new[] { "AAVE", "AVAX", "CAKE", "COMP", "JST", "KAVA", "LINK", "LUNA", "MKR", "OCEAN", "REEF", "SOL", "SWAP", "UNI", "YFI" });

            if (currencies.HasFlag(CryptoCategory.Gen2d))
                list.AddRange(new[] { "ATOM", "BTT", "FIL", "THETA", "VET", "ZRX" });

            if (currencies.HasFlag(CryptoCategory.Fiat))
                list.AddRange(new[] { "USD", "UAH", "EUR", "RUB", "AUD", "CAD", "GBP", "INR", "IDR", "PHP", "SGD", "SNI" });

            if (currencies.HasFlag(CryptoCategory.LeveragedToken))
                list.AddRange(new[] {
                    "ADABULL", "BULL", "BCHBULL","ETHBULL","EOSBULL","LINKBULL","TRXBULL", "XRPBULL",
                    "ADABEAR", "BEAR", "BCHBEAR","ETHBEAR","EOSBEAR","LINKBEAR","TRXBEAR", "XRPBEAR"});

            return list.Distinct().ToArray();
        }

        public string[] GetBaseCurrencies(bool all = true)
        {
            return all ?
                new[] { "BTC", "BNB", "USDT", "USDC", "BUSD", "TUSD", "ETH", "USD", "UAH", "RUB", "PAX", "VAI", "EUR", "GBP", "CAD" } :
                new[] { "BTC", "USDT", "USD", "UAH" };
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
                            res.AddRange(TradingPairHelper.Combine("BTC", GetCurrencies(CryptoCategory.Top)));
                            break;
                        }
                    case "USDT":
                        {
                            res.AddRange(TradingPairHelper.Combine("USDT",
                                GetCurrencies(CryptoCategory.AllCrypto)));
                            break;
                        }
                    case "BUSD":
                        {
                            res.AddRange(TradingPairHelper.Combine("BUSD",
                                GetCurrencies(CryptoCategory.Top & CryptoCategory.StableCoin)));
                            break;
                        }
                    case "USDC":
                        {
                            res.AddRange(TradingPairHelper.Combine("USDC", "BTC", "BCH", "DASH", "DOGE", "ETH", "LTC", "XEM",
                                "XLM", "XMR", "XRP"));
                            break;
                        }
                    case "USD":
                        {
                            res.AddRange(TradingPairHelper.Combine("USD", "BTC", "BCH", "DASH", "DOGE", "ETH", "LTC", "XEM",
                                "XLM", "XMR", "XRP", "USDT", "USDC", "ZEC"));
                            break;
                        }
                    case "UAH":
                        {
                            res.AddRange(TradingPairHelper.Combine("UAH", "BTC", "BCH", "DASH", "ETH", "LTC", "XEM", "XLM",
                                "XRP", "USDT", "ZEC"));
                            break;
                        }
                }
            }

            return res.ToArray();
        }

        public string[] GetTopSymbols(params string[] quoteCurrencies)
        {
            var res = new List<string>();

            foreach (var quote in quoteCurrencies)
            {
                switch (quote)
                {
                    case "BTC":
                        {
                            res.AddRange(TradingPairHelper.Combine("BTC", GetCurrencies(CryptoCategory.Top)));
                            break;
                        }
                    case "USDT":
                        {
                            res.AddRange(TradingPairHelper.Combine("USDT",
                                GetCurrencies(CryptoCategory.AllCrypto)));
                            break;
                        }
                    case "BUSD":
                        {
                            res.AddRange(TradingPairHelper.Combine("BUSD",
                                GetCurrencies(CryptoCategory.Top & CryptoCategory.StableCoin)));
                            break;
                        }
                    case "USDC":
                        {
                            res.AddRange(TradingPairHelper.Combine("USDC", "BTC", "BCH", "DASH", "DOGE", "ETH", "LTC", "XEM",
                                "XLM", "XMR", "XRP"));
                            break;
                        }
                    case "USD":
                        {
                            res.AddRange(TradingPairHelper.Combine("USD", "BTC", "BCH", "DASH", "DOGE", "ETH", "LTC", "XEM",
                                "XLM", "XMR", "XRP", "USDT", "USDC", "ZEC"));
                            break;
                        }
                    case "UAH":
                        {
                            res.AddRange(TradingPairHelper.Combine("UAH", "BTC", "BCH", "DASH", "ETH", "LTC", "XEM", "XLM",
                                "XRP", "USDT", "ZEC"));
                            break;
                        }
                }
            }

            return res.ToArray();
        }
    }
}