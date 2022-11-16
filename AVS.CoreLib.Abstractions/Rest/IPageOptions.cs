namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IPageOptions
    {
        int Limit { get; set; }
        int Offset { get; set; }
        string Sort { get; set; }
    }
}