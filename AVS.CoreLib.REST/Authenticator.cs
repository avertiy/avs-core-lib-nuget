using System.Security.Cryptography;
using System.Text;
using AVS.CoreLib.Abstractions.Rest;

namespace AVS.CoreLib.REST
{
    public class Authenticator<TAlgorithm> : IAuthenticator
        where TAlgorithm : KeyedHashAlgorithm, new()
    {
        private object _lock = new();
        public string PublicKey { get; private set; }
        public string PrivateKey { get; private set; }

        public Encoding Encoding { get; set; } = Encoding.ASCII;
        protected TAlgorithm Encryptor { get; }

        public Authenticator()
        {
            Encryptor = new TAlgorithm();
        }

        public Authenticator(string publicKey, string privateKey)
        {
            Encryptor = new TAlgorithm();
            SetKeys(publicKey, privateKey);
        }

        public void SetKeys(string publicKey, string privateKey)
        {
            if(string.IsNullOrEmpty(privateKey))
                return;

            if(PublicKey == publicKey)
                return;

            PublicKey = publicKey;
            PrivateKey = privateKey;
            Encryptor.Key = Encoding.GetBytes(privateKey);
        }

        public virtual byte[] ComputeHash(byte[] bytes)
        {
            lock (_lock)
            {
                return Encryptor.ComputeHash(bytes);
            }
        }

        public virtual byte[] Sign(string payload)
        {
            var postBytes = Encoding.GetBytes(payload);
            return ComputeHash(postBytes);
        }

        public virtual byte[] Sign(string payload, out byte[] bytes)
        {
            bytes = Encoding.GetBytes(payload);
            return ComputeHash(bytes);
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