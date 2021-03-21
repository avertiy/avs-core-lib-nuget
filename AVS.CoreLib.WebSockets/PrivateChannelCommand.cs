using System.Runtime.Serialization;

namespace AVS.CoreLib.WebSockets
{
    [DataContract]
    public class PrivateChannelCommand : ChannelCommand
    {
        [DataMember(Name = "key")]
        public string Key { get; set; }
        [DataMember(Name = "payload")]
        public string Payload { get; set; }
        [DataMember(Name = "sign")]
        public string Signature { get; set; }
    }
}