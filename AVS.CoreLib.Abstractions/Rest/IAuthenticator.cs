using System.Text;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IAuthenticator
    {
        Encoding Encoding { get; set; }
        string PublicKey { get; }
        string? Password { get; }
        /// <summary>
        /// switch (update) api keys for example to switch between accounts using the same client
        /// </summary>
        void SetKeys(string publicKey, string privateKey, string? password = null);
        /// <summary>
        /// sign data an returns signature hash
        /// </summary>
        byte[] Sign(string payload);

        byte[] Sign(string payload, out byte[] bytes);

        byte[] ComputeHash(byte[] bytes);
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member