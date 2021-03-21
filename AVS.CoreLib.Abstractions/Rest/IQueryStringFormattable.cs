namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IQueryStringFormattable
    {
        string ToQueryString(string format);
    }
}