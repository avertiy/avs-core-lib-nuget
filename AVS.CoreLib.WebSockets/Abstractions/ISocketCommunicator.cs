using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.WebSockets.Abstractions
{
    /// <summary>
    ///  An abstraction layer over <see cref=" System.Net.WebSockets.ClientWebSocket"/>
    ///  SocketCommunicator encapsulates low-level websocket routine:
    ///   - opening web socket connection,
    ///   - managing web socket state,
    ///   - background thread for an infinite loop of receiving messages
    ///  Fire events:
    ///     - ConnectionClosed,
    ///     - ConnectionError,
    ///     - MessageArrived (sync version)
    ///     - MessageArrivedAsync (async is invoked with Task.Run to avoid blocking the listening thread) 
    /// </summary>
    public interface ISocketCommunicator : IDisposable
    {
        /// <summary>
        /// refers to underlying <see cref="ClientWebSocket.Options"/> 
        /// </summary>
        ClientWebSocketOptions Options { get; }

        WebSocketState State { get; }

        /// <summary>
        /// connect to websocket channel at given uri
        /// </summary>
        Task<bool> ConnectAsync(Uri uri, CancellationToken ct);

        /// <summary>
        /// Ensure websocket <see cref="State"/> is open, convert given command/message to bytes and send them to websocket channel
        /// </summary>
        Task SendAsync(string message, CancellationToken cancellationToken);

        /// <summary>
        /// dispose existing underlying websocket client and create a fresh one
        /// </summary>
        void ResetWebSocket();

        event Action<string> MessageArrived;
        event Action<string> MessageArrivedAsync;
        event Action<string> ConnectionClosed;
        event Action<Exception> ConnectionError;
    }
}