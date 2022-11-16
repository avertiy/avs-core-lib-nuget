namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IQueryStringFormatable
    {
        string ToQueryString(string format);
    }
}