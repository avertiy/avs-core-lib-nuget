#nullable enable
using AVS.CoreLib.Trading.Enums.TA;

namespace AVS.CoreLib.Trading.Abstractions.Bars
{
    public interface IBollingerBands : IBarExtension
    {
        decimal Basis { get; set; }
        decimal UpperBand { get; set; }
        decimal LowerBand { get; set; }
        decimal UpperNarrowBand { get; set; }
        decimal LowerNarrowBand { get; set; }
        MAColor Color { get; set; }
    }
}