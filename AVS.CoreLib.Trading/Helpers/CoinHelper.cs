using System.Collections.Generic;

namespace AVS.CoreLib.Trading.Helpers
{
    public static class CoinHelper
    {
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
            return Stablecoins.IsStableCoin(isoCode);
        }

        public static bool IsFiatCurrency(string iso)
        {
            switch (iso)
            {
                case "USD":
                case "EUR":
                case "UAH":
                case "RUB":
                case "GBP":
                case "AUD":
                case "CAD":
                case "SGD":
                case "INR":
                case "IDR":
                case "SNI":
                case "PHP":
                    return true;
                default:
                    return false;
            }
        }

        public static void RegisterAlias(string alias, string isoCode)
        {
            Aliases.Add(alias, isoCode);
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


    public static class Stablecoins
    {
        public static List<string> GetAll()
        {
            var list = new List<string>()
            {
                "USDT",
                "USDC",
                "BUSD",
                "DAI",
                "TUSD",
                "PAX"
            };
            return list;
        }

        public static bool IsStableCoin(string iso)
        {
            switch (iso)
            {
                case "USDT":
                case "USDC":
                case "BUSD":
                case "TUSD":
                case "DAI":
                case "PAX":
                    return true;
                default:
                    return false;
            }
        }
    }
}