using System;
using System.Globalization;
using System.Numerics;
using AVS.CoreLib.Dates;

namespace AVS.CoreLib.Utilities
{
    public static class NonceHelper
    {
        private static BigInteger CurrentHttpPostNonce { get; set; }
        public static string GetNonce()
        {
            var totalms = DateTime.UtcNow.Subtract(UnixEpoch.Start).TotalMilliseconds;
            var newHttpPostNonce = new BigInteger(Math.Round(totalms * 1000, MidpointRounding.AwayFromZero));
            if (newHttpPostNonce > CurrentHttpPostNonce)
            {
                CurrentHttpPostNonce = newHttpPostNonce;
            }
            else
            {
                CurrentHttpPostNonce += 1;
            }

            return CurrentHttpPostNonce.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// tonce the same as nonce but looks a bit (for 2-3 digits) shorter than nonce 
        /// </summary>
        public static string GetTonce()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
        }
    }
}