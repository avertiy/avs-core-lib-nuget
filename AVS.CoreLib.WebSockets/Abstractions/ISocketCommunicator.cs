using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.WebSockets.Abstractions
{
    /// <summary>
    /// Wrapper for low level communication details for ClientWebSocket
    /// </summary>
    public interface ISocketCommunicator : IDisposable
    {
        TimeSpan KeepAliveInterval { get; set; }
        ClientWebSocketOptions Options { get; }
        WebSocketState State { get; }

        Task<bool> ConnectAsync(Uri uri, CancellationToken cancellationToken);

        Task SendAsync(string commandMessage, CancellationToken cancellationToken);

        void ResetWebSocket();

        event Action<string> MessageArrived;
        event Action<string> MessageArrivedAsync;
        event Action ConnectionClosed;
        event Action<Exception> ConnectionError;
    }
}