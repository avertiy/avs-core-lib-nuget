using System.Security.Cryptography;
using System.Text;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.REST.Extensions;

namespace AVS.CoreLib.REST
{
    public class Authenticator<TAlgorithm> : IAuthenticator
        where TAlgorithm : KeyedHashAlgorithm, new()
    {
        public string PublicKey { get; private set; }
        public Encoding Encoding = Encoding.ASCII;
        protected TAlgorithm Encryptor { get; }

        public Authenticator(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            Encryptor = new TAlgorithm();
            if (!string.IsNullOrEmpty(privateKey))
                Encryptor.Key = Encoding.GetBytes(privateKey);
        }


        public void SwitchKeys(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            Encryptor.Key = Encoding.GetBytes(privateKey);
        }

        public byte[] GetBytes(string postData, out string signature)
        {
            byte[] postBytes = Encoding.GetBytes(postData);
            signature = Encryptor.ComputeHash(postBytes).ToStringHex();
            return postBytes;
        }

        public string Sign(string message)
        {
            byte[] postBytes = Encoding.GetBytes(message);
            return Encryptor.ComputeHash(postBytes).ToStringHex();
        }
    }

    public class HMACSHA512Authenticator : Authenticator<HMACSHA512>
    {
        public HMACSHA512Authenticator(string publicKey, string privateKey) : base(publicKey, privateKey)
        {
        }
    }

    public class HMACSHA256Authenticator : Authenticator<HMACSHA256>
    {
        public HMACSHA256Authenticator(string publicKey, string privateKey) : base(publicKey, privateKey)
        {
        }
    }
}