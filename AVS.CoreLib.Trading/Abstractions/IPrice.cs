namespace AVS.CoreLib.Trading.Abstractions
{
    public interface IPrice
    {
        decimal Price { get; }
    }

    public interface IMutPrice : IPrice
    {
        new decimal Price { get; set; }
    }
}