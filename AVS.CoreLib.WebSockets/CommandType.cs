using System.Runtime.Serialization;

namespace AVS.CoreLib.WebSockets
{
    public enum CommandType
    {
        //[EnumMember(Value = "private")]
        //Private,
        [EnumMember(Value = "unsubscribe")]
        Unsubscribe,
        [EnumMember(Value = "subscribe")]
        Subscribe
    }
}