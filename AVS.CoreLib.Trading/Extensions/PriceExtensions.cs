using System;
using AVS.CoreLib.Extensions.Numbers;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class PriceExtensions
    {
        public static int GetRoundDecimals(this double price)
        {
            return price switch
            {
                > 100 => 2,
                > 10 => 3,
                > 1 => 4,
                > 0.1 => 5,
                _ => 6
            };
        }

        public static double PriceRoundUp(this double price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetRoundDecimals(price);
            return price.RoundUp(dec);
        }

        public static double PriceRoundDown(this double price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetRoundDecimals(price);
            return price.RoundDown(dec);
        }
    }

    public static class VolumeExtensions
    {
        public static int GetVolumeRoundDecimals(this double price)
        {
            //on Binance volume round decimals looks as the following:
            //price 5 000$ => 5
            //price > 50$ => 3
            //price > 2$ => 2
            //price > 1$ => 1
            // 0
            return price switch
            {
                > 5_000 => 5,
                > 500 => 4,
                > 50 => 3,
                > 2 => 2,
                > 1 => 1,
                _ => 0
            };
        }

        public static double VolumeRoundUp(this double volume, double price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetVolumeRoundDecimals(price);
            return volume.RoundUp(dec);
        }

        public static double VolumeRoundDown(this double volume, double price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetVolumeRoundDecimals(price);
            return volume.RoundDown(dec);
        }
    }
}