using System;
using System.Threading.Tasks;

namespace AVS.CoreLib.WebSockets.Abstractions
{
    /// <summary>
    /// Channel manager provides interface to subscribe/unsubscribe on a socket channel/stream
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

        /// <summary>
        /// Send command (usually a subscribe command) to subscribe on a certain websocket channel/stream.
        /// </summary>
        /// <remarks>
        /// The websocket url is taken from <see cref="BaseAddress"/>. 
        /// Use <see cref="SetMessageHandler"/> to handle incoming messages.
        /// </remarks>
        /// <param name="channelName">channel name, it is used to keep track channels list to auto-reconnect</param>
        /// <param name="command">command or message to subscribe on the channel/stream</param>
        /// <param name="autoReconnect">indicates whether to auto-reconnect to the chanel in case the websocket closed event occurs</param>
        Task Subscribe(string channelName, string command, bool autoReconnect = false);

        Task UnSubscribe(string channelName, string command);

        void SetMessageHandler(Action<string> handler);
        void SetConnectionClosedHandler(Action<string> handler);
        void Reset();
    }
}