#nullable enable

using AVS;

namespace AVS.CoreLib.Trading.Abstractions.TA
{
    public interface IValue
    {
        decimal Value { get; set; }
    }

    //public interface IMAValue : IValue
    //{
    //    decimal Distance { get; set; }
    //    /// <summary>
    //    /// MA slope indicates trend direction and bull/bear strength
    //    /// </summary>
    //    Slope Slope { get; set; }
    //}

    //public interface IBBValue : IMAValue
    //{
    //    decimal UpperBand { get; set; }
    //    decimal LowerBand { get; set; }
    //    decimal UpperNarrowBand { get; set; }
    //    decimal LowerNarrowBand { get; set; }
    //}
}