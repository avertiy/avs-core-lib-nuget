using System.Text;

namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IAuthenticator
    {
        Encoding Encoding { get; set; }
        string PublicKey { get; }
        /// <summary>
        /// switch (update) api keys for example to switch between accounts using the same client
        /// </summary>
        void SetKeys(string publicKey, string privateKey);
        /// <summary>
        /// sign data an returns signature hash
        /// </summary>
        byte[] Sign(string payload);

        byte[] Sign(string payload, out byte[] bytes);

        byte[] Sign(byte[] payload);
    }    
}