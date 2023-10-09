#nullable enable

using AVS;

namespace AVS.CoreLib.Trading.Abstractions.TA
{
    public interface IValue
    {
        decimal Value { get; set; }
    }    
}