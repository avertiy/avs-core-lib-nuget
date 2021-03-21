namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IAuthenticator
    {
        string PublicKey { get; }
        void SwitchKeys(string publicKey, string privateKey);
        byte[] GetBytes(string postData, out string signature);
        string Sign(string message);
    }
}