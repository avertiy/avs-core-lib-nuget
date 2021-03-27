using System;

namespace AVS.CoreLib.Trading.Helpers
{
    public interface IRankHelper
    {
        /// <summary>
        /// estimates trade total rank
        /// was it a micro trade or a large one
        /// the greater rank the larger the trade was
        /// </summary>
        int GetRank(decimal total, string baseCurrency);
    }

    public class RankHelper : IRankHelper
    {
        public const int MaxRank = 12;
        public int GetRank(decimal total, string baseCurrency)
        {
            var rank = 0;
            switch (baseCurrency)
            {
                case "AUD":
                case "CHF":
                case "USD":
                case "EUR":
                case "GBP":
                case "USDC":
                case "USDT":
                case "BUSD":
                case "TUSD":
                case "DAI":
                    {
                        rank = GetRankForUSD(total);
                        break;
                    }
                case "RUB":
                case "UAH":
                    {
                        rank = GetRankForUSD(total / 25);
                        break;
                    }
                case "BTC":
                    {
                        rank = GetRankForBTC(total);
                        break;
                    }
                case "ETH":
                    {
                        rank = GetRankForBTC(total * 0.03m);
                        break;
                    }
                default:
                    throw new NotSupportedException($"{baseCurrency} not supported");
            }

            return rank;
        }

        private int GetRankForUSD(in decimal total)
        {
            var rank = 0;
            if (total <= 100)
                rank = 0;
            else if (total < 500)
                rank = 1;
            else if (total < 1000)
                rank = 2;
            else if (total < 2500)
                rank = 3;
            else if (total < 5000)
                rank = 4;
            else if (total < 10000)
                rank = 5;
            else if (total < 25000)
                rank = 6;
            else if (total < 50000)
                rank = 7;
            else if (total < 100000)
                rank = 8;
            else if (total < 250000)
                rank = 9;
            else if (total < 500000)
                rank = 10;
            else if (total < 1000000)
                rank = 11;
            else
                rank = 12;
            return rank;
        }

        private int GetRankForBTC(in decimal total)
        {
            var rank = 0;
            if (total <= 0.01m)
                rank = 0;
            else if (total < 0.1m)
                rank = 1;
            else if (total < 0.5m)
                rank = 2;
            else if (total < 1)
                rank = 3;
            else if (total < 5)
                rank = 4;
            else if (total < 10)
                rank = 5;
            else if (total < 50)
                rank = 6;
            else if (total < 100)
                rank = 7;
            else if (total < 250)
                rank = 8;
            else if (total < 500)
                rank = 9;
            else
                rank = 10;
            return rank;
        }
    }
}