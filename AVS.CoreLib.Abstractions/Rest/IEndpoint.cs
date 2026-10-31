#pragma warning disable CS1591
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

    public enum QueryMode
    {
        Default = 0,
        /// <summary>
        /// query "hot" live data
        /// </summary>
        Live = 1,
        /// <summary>
        /// query "cold" historical data
        /// </summary>
        Historical =2
    }
}

#pragma warning restore CS1591