namespace AVS.CoreLib.Trading.Abstractions
{
    public interface ISymbol
    {
        string Symbol { get; }
    }

    public interface IMutSymbol : ISymbol
    {
        new string Symbol { get; set; }
    }
}