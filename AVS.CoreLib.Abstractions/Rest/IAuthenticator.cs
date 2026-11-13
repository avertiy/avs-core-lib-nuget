using System;
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

    public static class AuthenticatorExtensions
    {
        public static string GetSignature(this IAuthenticator authenticator, string payload, EncodingType encodingType = EncodingType.Base64)
        {
            var bytes = authenticator.Sign(payload);
            var signature = encodingType == EncodingType.Base64
                ? Convert.ToBase64String(bytes)
                : Convert.ToHexString(bytes);
            return signature;
        }
    }

    public enum EncodingType
    {
        Base64 = 0,
        Hex = 1,
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member