using System;
using System.Threading.Tasks;

namespace AVS.CoreLib.WebSockets.Abstractions
{
    /// <summary>
    /// Channel manager provides interface to subscribe/unsubscribe on a socket channel/stream
    /// 
    /// An abstraction layer on websocket client (<see cref="IWebSocketClient"/>),
    /// manage  dealing with a certain websocket api url, subscribing on channel(s)/stream(s)
    /// </summary>
    public interface IChannelManager : IDisposable
    {
        /// <summary>
        /// the address is used as uri to connect to websocket channel
        /// </summary>
        string BaseAddress { get; set; }
        bool AutoReconnect { get; set; }
        IWebSocketClient Client { get; }

        event Action<string> ConnectionClosed;

        Task Subscribe(string channelName, string command, bool autoReconnect = false);

        Task UnSubscribe(string channelName, string command);

        void ClearChannels();
    }

    public interface IMessageProcessor
    {
        void ProcessMessage(string message);
    }
}