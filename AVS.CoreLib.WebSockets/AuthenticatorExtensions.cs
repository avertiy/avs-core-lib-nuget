using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.WebSockets
{
    public static class AuthenticatorExtensions
    {
        /// <summary>
        /// fills in key, signature and nonce payload 
        /// </summary>
        public static void Sign(this IAuthenticator authenticator, PrivateChannelCommand cmd)
        {
            cmd.Payload = $"nonce={NonceHelper.GetNonce()}";
            cmd.Key = authenticator.PublicKey;
            cmd.Signature = authenticator.Sign(cmd.Payload);
        }
    }
}