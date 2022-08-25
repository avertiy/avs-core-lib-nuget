using System.Security.Cryptography;
using System.Text;
using AVS.CoreLib.Abstractions.Rest;

namespace AVS.CoreLib.REST
{
    public class Authenticator<TAlgorithm> : IAuthenticator
        where TAlgorithm : KeyedHashAlgorithm, new()
    {
        public string PublicKey { get; private set; }
        public Encoding Encoding { get; set; } = Encoding.ASCII;
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

        public virtual byte[] Sign(byte[] bytes)
        {
            return Encryptor.ComputeHash(bytes);
        }

        public virtual byte[] Sign(string payload)
        {
            var postBytes = Encoding.GetBytes(payload);
            return Encryptor.ComputeHash(postBytes);
        }

        public virtual byte[] Sign(string payload, out byte[] bytes)
        {
            bytes = Encoding.GetBytes(payload);
            return Encryptor.ComputeHash(bytes);
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