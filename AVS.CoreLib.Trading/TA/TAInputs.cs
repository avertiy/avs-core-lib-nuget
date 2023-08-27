#nullable enable
using System;

namespace AVS.CoreLib.Trading.TA
{
    public class TAInputs
    {
        public int MovingStats { get; set; } = 12;
        public int[] SMA { get; set; } = new[] { 21, 50, 100, 200 };
        public int[] EMA { get; set; } = new[] { 6, 13, 21 };
        public int[] HMA { get; set; } = new[] { 21, 200 };

        public int[] RSI { get; set; } = new[] { 12, 21 };
        public int[] ADX { get; set; } = new[] { 14 };
        public int[] BB { get; set; } = new[] { 21 };

        public (int, int, int)[] MACD { get; set; } = new[] { (12, 26, 9) };

        public (int, int, int)[] Stoch { get; set; } = new[] { (14, 3, 5), (42, 3, 5) };

        public decimal StdDevMul { get; set; } = 2.5m;
        public decimal StdDevNarrowMul { get; set; } = 1;
    }    
}