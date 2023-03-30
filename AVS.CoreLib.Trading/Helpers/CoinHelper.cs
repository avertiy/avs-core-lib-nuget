using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Trading.Helpers
{
    public static class CoinHelper
    {
        public static List<string> StableCoins = new List<string>()
        {
            "USDT",
            "USDC",
            "BUSD",
            "USDJ",
            "TUSD",
            "USDD",
            "DAI",
            "VAI"
        };

        public static List<string> Fiat = new List<string>()
        {
            "USD",
            "UAH",
            "EUR",
            "RUB",
            "GBP",
            "AUD",
            "CAD",
            "SGD",
            "INR",
            "IDR",
            "SNI",
            "PHP"
        };

        /// <summary>
        /// BTC,ETH,BNB,XRP,TRX - i.e. symbols that might serve as quote asset in trading pairs
        /// </summary>
        public static List<string> Top = new List<string>()
        {
            "BTC",
            "ETH",
            "BNB",
            "XRP",
            "TRX"
        };

        private static Dictionary<string, string> _aliases;
        public static Dictionary<string, string> Aliases
        {
            get
            {
                if (_aliases == null)
                {
                    _aliases = new Dictionary<string, string>
                    {
                        { "STR", "XLM" },
                        { "DSH", "DASH" }
                    };
                }

                return _aliases;
            }
        }

        public static bool IsShitCoin(string isoCode)
        {
            switch (isoCode)
            {
                case "KICK":
                case "INK":
                case "MNX":
                case "PTI":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsStableCoin(string isoCode)
        {
            return StableCoins.Contains(isoCode);
        }

        public static bool IsFiatCurrency(string iso)
        {
            return Fiat.Contains(iso);
        }

        public static void RegisterAlias(string alias, string isoCode)
        {
            Aliases.Add(alias, isoCode);
        }

        public static bool MatchStablecoin(string symbol)
        {
            return StableCoins.Any(symbol.EndsWith);
        }

        public static bool MatchAlias(string pair, out string correctPair)
        {
            correctPair = pair;
            var parts = pair.Split("_");
            //var key1 = $"{exchange}:{parts[0]}";
            //var key2 = $"{exchange}:{parts[1]}";

            foreach (var key in Aliases.Keys)
            {
                if (key == parts[0])
                {
                    correctPair = $"{Aliases[key]}_{parts[1]}";
                    return true;
                }
                if (key == parts[1])
                {
                    correctPair = $"{parts[0]}_{Aliases[key]}";
                    return true;
                }
            }
            return false;
        }

        public static string GetCurrencySymbol(string isoCode)
        {
            switch (isoCode)
            {
                case "UAH":
                    return "₴";
                case "USD":
                case "USDT":
                case "USDC":
                case "BUSD":
                case "TUSD":
                case "USDX":
                case "DAI":
                    return "$";
                case "EUR":
                    return "€";
                case "RUB":
                    return "₽";
                case "BTC":
                    return "₿";
                case "LTC":
                    return "Ł";
                case "ETH":
                    return "E";
                case "DOGE":
                    return "Ɖ";
                default:
                    return isoCode;
            }
        }
    }
}