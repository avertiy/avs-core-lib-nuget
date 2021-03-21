namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IPageOptions : IQueryStringFormattable
    {
        int Limit { get; set; }
        int Offset { get; set; }
        string Sort { get; set; }
    }
}