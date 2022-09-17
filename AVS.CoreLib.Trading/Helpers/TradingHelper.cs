using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Helpers
{
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
                            res.AddRange(PairHelper.GeneratePairs("BTC", GetCurrencies(CryptoCategory.Top)));
                            break;
                        }
                    case "USDT":
                        {
                            res.AddRange(PairHelper.GeneratePairs("USDT",
                                GetCurrencies(CryptoCategory.AllCrypto)));
                            break;
                        }
                    case "BUSD":
                        {
                            res.AddRange(PairHelper.GeneratePairs("BUSD",
                                GetCurrencies(CryptoCategory.Top & CryptoCategory.StableCoin)));
                            break;
                        }
                    case "USDC":
                        {
                            res.AddRange(PairHelper.GeneratePairs("USDC", "BTC", "BCH", "DASH", "DOGE", "ETH", "LTC", "XEM",
                                "XLM", "XMR", "XRP"));
                            break;
                        }
                    case "USD":
                        {
                            res.AddRange(PairHelper.GeneratePairs("USD", "BTC", "BCH", "DASH", "DOGE", "ETH", "LTC", "XEM",
                                "XLM", "XMR", "XRP", "USDT", "USDC", "ZEC"));
                            break;
                        }
                    case "UAH":
                        {
                            res.AddRange(PairHelper.GeneratePairs("UAH", "BTC", "BCH", "DASH", "ETH", "LTC", "XEM", "XLM",
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
                            res.AddRange(PairHelper.GenerateSymbols("BTC", GetCurrencies(CryptoCategory.Top)));
                            break;
                        }
                    case "USDT":
                        {
                            res.AddRange(PairHelper.GenerateSymbols("USDT",
                                GetCurrencies(CryptoCategory.AllCrypto)));
                            break;
                        }
                    case "BUSD":
                        {
                            res.AddRange(PairHelper.GenerateSymbols("BUSD",
                                GetCurrencies(CryptoCategory.Top & CryptoCategory.StableCoin)));
                            break;
                        }
                    case "USDC":
                        {
                            res.AddRange(PairHelper.GenerateSymbols("USDC", "BTC", "BCH", "DASH", "DOGE", "ETH", "LTC", "XEM",
                                "XLM", "XMR", "XRP"));
                            break;
                        }
                    case "USD":
                        {
                            res.AddRange(PairHelper.GenerateSymbols("USD", "BTC", "BCH", "DASH", "DOGE", "ETH", "LTC", "XEM",
                                "XLM", "XMR", "XRP", "USDT", "USDC", "ZEC"));
                            break;
                        }
                    case "UAH":
                        {
                            res.AddRange(PairHelper.GenerateSymbols("UAH", "BTC", "BCH", "DASH", "ETH", "LTC", "XEM", "XLM",
                                "XRP", "USDT", "ZEC"));
                            break;
                        }
                }
            }

            return res.ToArray();
        }
    }
}