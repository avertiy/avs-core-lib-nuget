namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IEndpoint
    {
        string Command { get; set; }
        string Url { get; }
        string Method { get; set; }
        AuthType AuthType { get; set; }
    }
    public enum AuthType
    {
        None = 0,
        ApiKey
    }
}