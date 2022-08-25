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
        /// <summary>
        /// authentication not required i.e. public endpoint
        /// </summary>
        None = 0,
        /// <summary>
        /// authentication required, i.e. private endpoint and request needs to be signed
        /// </summary>
        ApiKey
    }
}