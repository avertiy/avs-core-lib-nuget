namespace AVS.CoreLib.Trading.Abstractions
{
    public interface IExchange
    {
        string Exchange { get; }
    }

    public interface IMutExchange : IExchange
    {
        new string Exchange { get; set; }
    }    
}