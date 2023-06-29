#nullable enable
using System;
using AVS.CoreLib.Extensions;
using System.Linq;
using AVS.CoreLib.Trading.Abstractions.XBars;
using AVS.CoreLib.Trading.Enums.Bars;

namespace AVS.CoreLib.Trading.TA.Indicators
{
    public class BB : IBBValue
    {
        public decimal Average { get; set; }
        public decimal Distance { get; set; }
        public Color Color { get; set; }
        public decimal UpperBand { get; set; }
        public decimal LowerBand { get; set; }
        public decimal UpperNarrowBand { get; set; }
        public decimal LowerNarrowBand { get; set; }
    }

    public class BBCalculator : SMACalculator<BB>
    {
        public decimal StdDevMul { get; set; }
        public decimal StdDevNarrowMul { get; set; }

        public BBCalculator(int length, decimal stdDevMul = 2.5m, decimal stdDevNarrowMul = 1) : base(length)
        {
            StdDevNarrowMul = stdDevNarrowMul;
            StdDevMul =stdDevMul;
        }

        public override BB? Process(decimal price)
        {
            var bb = base.Process(price);

            if(bb == null)
                return null;

            var variance = Values.Average(x => (x - bb.Average) * (x - bb.Average));
            var stdDev = Convert.ToDecimal(Math.Sqrt((double)variance));

            var decPlaces = price.GetDecimalPlaces();
            var basis = bb.Average;

            bb.UpperBand = (basis + stdDev * StdDevMul).Round(decPlaces);
            bb.UpperNarrowBand = (basis + stdDev * StdDevNarrowMul).Round(decPlaces);

            bb.LowerBand =  (basis - stdDev * StdDevMul).Round(decPlaces);
            bb.LowerNarrowBand = (basis -stdDev * StdDevNarrowMul).Round(decPlaces);

            return bb;
        }
    }
}