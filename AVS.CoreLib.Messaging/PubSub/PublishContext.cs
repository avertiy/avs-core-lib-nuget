using AVS.CoreLib.Abstractions.Messaging.PubSub;

namespace AVS.CoreLib.Messaging.PubSub
{
    public class PublishContext : IPublishContext
    {
        public bool Mandatory { get; set; }
        public bool Isolated { get; set; }
    }
}