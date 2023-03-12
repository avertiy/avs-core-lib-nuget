using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class TradingEnumsExtensions
    {
        public static CoinCap GetCoinCapByRank(this int rank)
        {
            return rank switch
            {
                <= 0 => CoinCap.None,
                > 0 and < 50 => CoinCap.BigCap,
                > 50 and < 150 => CoinCap.MidCap,
                > 150 and < 375 => CoinCap.SmallCap,
                > 375 and < 1200 => CoinCap.MicroCap,
                _ => CoinCap.SeedCap
            };
        }
    }
}