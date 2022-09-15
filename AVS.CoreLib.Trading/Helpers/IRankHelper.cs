using System;

namespace AVS.CoreLib.Trading.Helpers
{
    public interface IRankHelper
    {
        int MaxRank { get; }
        /// <summary>
        /// estimates trade total rank
        /// was it a micro trade or a large one
        /// the greater rank the larger the trade was
        /// </summary>
        int GetRank(decimal total, string quoteCurrency);

    }

    public class RankHelper : IRankHelper
    {
        public virtual int MaxRank { get; } = 10;

        public int GetRank(decimal total, string quoteCurrency)
        {
            var rank = 0;
            switch (quoteCurrency)
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
                    throw new NotSupportedException($"{quoteCurrency} not supported");
            }

            return rank;
        }

        protected virtual int GetRankForBTC(in decimal total)
        {
            var rank = 0;
            if (total < 0.02m)
                rank = 0;
            else if (total < 0.075m)
                rank = 1;
            else if (total < 0.25m)
                rank = 2;
            else if (total < 1)
                rank = 3;
            else if (total < 5)
                rank = 4;
            else if (total < 10)
                rank = 5;
            else if (total < 50)
                rank = 6;
            else if (total < 200)
                rank = 7;
            else if (total < 500)
                rank = 8;
            else
                rank = 9;
            return rank;
        }

        protected virtual int GetRankForUSD(in decimal total)
        {
            var rank = 0;
            if (total < 1000)
                rank = 0;
            else if (total < 3000)
                rank = 1;
            else if (total < 10_000)
                rank = 2;
            else if (total < 30_000)
                rank = 3;
            else if (total < 100_000)
                rank = 4;
            else if (total < 250_000)
                rank = 5;
            else if (total < 1_000_000)
                rank = 6;
            else if (total < 5_000_000)
                rank = 7;
            else if (total < 10_000_000)
                rank = 8;
            else
                rank = 9;
            return rank;
        }
    }

    public class AbcRankHelper : RankHelper
    {
        public override int MaxRank { get; } = 3;

        protected override int GetRankForBTC(in decimal total)
        {
            var rank = 0;
            if (total <= 1)
                rank = 0;
            else if (total <= 10)
                rank = 1;
            else if (total >= 10)
                rank = 2;
            return rank;
        }

        protected override int GetRankForUSD(in decimal total)
        {
            var rank = 0;
            if (total <= 50_000)
                rank = 0;
            else if (total <= 500_000)
                rank = 1;
            else if (total > 500_000)
                rank = 2;
            return rank;
        }
    }
}