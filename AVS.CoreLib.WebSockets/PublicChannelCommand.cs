using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AVS.CoreLib.WebSockets
{
    [DataContract]
    public class ChannelCommand : IChannelCommand
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember(Name = "command")]
        public CommandType Command { get; set; }

        [DataMember(Name = "channel")]
        public int Channel { get; set; }

        [DataMember(Name = "id")]
        public int? Id { get; set; }

        public virtual string ToJsonMessage()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }

    [DataContract]
    public class PublicChannelCommand : ChannelCommand
    {
    }
}