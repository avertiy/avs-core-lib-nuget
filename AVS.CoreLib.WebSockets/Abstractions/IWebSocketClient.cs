using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace AVS.CoreLib.WebSockets.Abstractions
{
    public interface IWebSocketClient : IDisposable
    {
        bool IsConnected { get; }
        WebSocketState State { get; }
        ClientWebSocketOptions Options { get; }
        bool DiagnosticEnabled { get; set; }
        Uri Uri { get; set; }
        TimeSpan KeepAliveInterval { get; set; }

        event Action<Exception> ConnectionClosed;
        event Action<string> MessageArrived;

        Task SendAsync(string command);

        Task ReconnectAsync();
    }
}